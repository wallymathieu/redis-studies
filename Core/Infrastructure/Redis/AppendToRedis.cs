using System.Linq;
using With;
using System.Collections.Generic;
using StackExchange.Redis;
using SomeBasicFileStoreApp.Core.Commands;
using System;

namespace SomeBasicFileStoreApp.Core.Infrastructure.Redis
{
    public class AppendToRedis
    {
        private readonly IDatabase db;
        public AppendToRedis(IDatabase db)
        {
            this.db = db;
        }
        public void Batch(IEnumerable<Command> commands)
        {
            var batch = this.db.CreateBatch();
            var ids = new List<Guid>();
            foreach (var command in commands)
            {
                ids.Add(command.HashCreate(batch));
            }
            batch.SetAddAsync("Commands", ids.Select(id => (RedisValue)id.ToString()).ToArray());
            batch.Execute();
        }

        public IEnumerable<Command> ReadAll()
        {
            return ReadAll(db);
        }

        public static Command Read(IDatabase db, Guid key)
        {
            var entries = db.HashGetAll(key);
            var type = entries.GetString("Type");
            var command = RedisExtensions.getCommand[type];
            var instance = (Command)Activator.CreateInstance(command);
            instance.SequenceNumber = entries.GetInt("SequenceNumber");
            Switch.On(instance)
                .Case((AddCustomerCommand c) => AddCustomerCommandMap.Restore(c, db, key))
                .Case((AddOrderCommand c) => AddOrderCommandMap.Restore(c, db, key))
                .Case((AddProductCommand c) => AddProductCommandMap.Restore(c, db, key))
                .Case((AddProductToOrder c) => AddProductToOrderMap.Restore(c, db, key))
                .ElseFail()
                ;
            return instance;
        }

        public static IEnumerable<Command> ReadAll(IDatabase db)
        {
            var commands = db.SetMembers("Commands");
            foreach (var item in commands)
            {
                yield return Read(db, Guid.Parse(item));
            }
        }
    }

}