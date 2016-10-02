using System.Linq;
using With;
using System.Collections.Generic;
using StackExchange.Redis;
using SomeBasicFileStoreApp.Core.Commands;
using System;
using System.Threading.Tasks;

namespace SomeBasicFileStoreApp.Core.Infrastructure.Redis
{
    public class AppendToRedis : IAppendBatch, IReadBatch
    {
        private readonly IDatabase db;
        public AppendToRedis(IDatabase db)
        {
            this.db = db;
        }
        public async Task<Guid[]> Batch(IEnumerable<Command> commands)
        {
            var batch = db.CreateBatch();
            var ids = new List<Task<Guid>>();
            foreach (var command in commands)
            {
                ids.Add(HashCreate(command, batch));
            }
            var redisValueIds = commands.Select(c => (RedisValue)c.UniqueId.ToString()).ToArray();
            var res = batch.SetAddAsync("Commands", redisValueIds);
            batch.Execute();
            await Task.WhenAll(ids);
            await res;
            return commands.Select(c => c.UniqueId).ToArray();
        }

        private static async Task<Guid> HashCreate(Command command, IBatch batch)
        {
            var id = command.UniqueId;
            var addBasic = batch.HashSetAsync(id.ToString(), new[]
                {
                    new HashEntry("Type", RedisExtensions.getName[command.GetType()]),
                    new HashEntry("SequenceNumber", command.SequenceNumber)
                });

            var hash = Switch.Match<Command, HashEntry[]>(command)
                .Case((AddCustomerCommand c) => AddCustomerCommandMap.ToHash(c))
                .Case((AddOrderCommand c) => AddOrderCommandMap.ToHash(c))
                .Case((AddProductCommand c) => AddProductCommandMap.ToHash(c))
                .Case((AddProductToOrder c) => AddProductToOrderMap.ToHash(c))
                .Value()
                ;
            var addSpecific = batch.HashSetAsync(id.ToString(), hash);
            await Task.WhenAll(new Task[] { addBasic, addSpecific });
            return id;
        }

        public async Task<IEnumerable<Command>> ReadAll()
        {
            return await ReadAll(db);
        }

        public async Task<IEnumerable<Command>> Read(Guid[] keys) 
        {
            var batch = db.CreateBatch();
            var res = Task.WhenAll(keys.Select(key =>
                                 Read(batch, key)
                                              ).ToArray());
            batch.Execute();
            return await res;
        }

        private static async Task<Command> Read(IBatch db, Guid key)
        {
            var entries = db.HashGetAllAsync(key.ToString());
            var hash = db.HashGetAllAsync(key.ToString());
            var e = await entries;
            var h = await hash;
            var type = e.GetString("Type");
            var command = RedisExtensions.getCommand[type];
            var instance = (Command)Activator.CreateInstance(command, new object[] { key });
            instance.SequenceNumber = e.GetInt("SequenceNumber");

            Switch.On(instance)
                .Case((AddCustomerCommand c) => AddCustomerCommandMap.FromHash(c, h))
                .Case((AddOrderCommand c) => AddOrderCommandMap.FromHash(c, h))
                .Case((AddProductCommand c) => AddProductCommandMap.FromHash(c, h))
                .Case((AddProductToOrder c) => AddProductToOrderMap.FromHash(c, h))
                .ElseFail()
                ;
            return instance;
        }

        private static async Task<Command[]> ReadAll(IDatabase db)
        {
            var commands = await db.SetMembersAsync("Commands");
            var batch = db.CreateBatch();
            var res = Task.WhenAll(commands.Select(item =>
                                   Read(batch, Guid.Parse(item))
                                                  ).ToArray());
            batch.Execute();
            return await res;
        }
    }

}