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
        public Guid[] Batch(IEnumerable<Command> commands)
        {
            var batch = db.CreateBatch();
            var ids = new List<Task<Guid>>();
            foreach (var command in commands)
            {
                ids.Add(command.HashCreate(batch));
            }
            var redisValueIds = commands.Select(c => (RedisValue)c.UniqueId.ToString()).ToArray();
            var res = batch.SetAddAsync("Commands", redisValueIds);
            batch.Execute();
            res.Wait();
            Task.WhenAll(ids);
            return commands.Select(c => c.UniqueId).ToArray();
        }

        public IEnumerable<Command> ReadAll()
        {
            return ReadAll(db).Result;
        }

        public IEnumerable<Command> Read(Guid[] keys) 
        {
            var batch = db.CreateBatch();
            var res = Task.WhenAll(keys.Select(key =>
                                 Read(batch, key)
                                              ).ToArray());
            batch.Execute();
            return res.Result;
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
                .Case((AddCustomerCommand c) => AddCustomerCommandMap.Restore(c, h))
                .Case((AddOrderCommand c) => AddOrderCommandMap.Restore(c, h))
                .Case((AddProductCommand c) => AddProductCommandMap.Restore(c, h))
                .Case((AddProductToOrder c) => AddProductToOrderMap.Restore(c, h))
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