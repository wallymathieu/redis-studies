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
        public void Batch(IEnumerable<Command> commands){
            var batch = this.db.CreateBatch();
            foreach (var command in commands)
            {
                Switch.On(command)
                    .Case((AddCustomerCommand c) => Persist(c,batch))
                    .Case((AddOrderCommand c)=> Persist(c,batch))
                    .Case((AddProductCommand c)=> Persist(c,batch))
                    .Case((AddProductToOrder c)=> Persist(c,batch))
                    .ElseFail()
                    ;
            }
            batch.Execute();
        }
        public IEnumerable<Command> ReadAll(){
            return ReadAll(db);
        }

        private static IDictionary<string,Type> getCommand;
        private static IDictionary<Type, string> getName;
        static AppendToRedis(){
            getCommand = new Dictionary<string,Type>{
                {"AddCustomerCommand", typeof(AddCustomerCommand)},
                {"AddOrderCommand"   , typeof(AddOrderCommand)},
                {"AddProductCommand" , typeof(AddProductCommand)},
                {"AddProductToOrder" , typeof(AddProductToOrder)},
            };
            getName = getCommand.ToDictionary(kv => kv.Value, kv => kv.Key);
        }

        protected static Guid HashCreate(Command command, IBatch batch)
        {
            var id = Guid.NewGuid();
            batch.SetAddAsync("Commands", id.ToString());
            batch.HashSetAsync(id.ToString(), new []
                {
                    new HashEntry("Type", getName[command.GetType()]),
                    new HashEntry("SequenceNumber", command.SequenceNumber)
                });
            return id;
        }

        public static Command Read(IDatabase db, Guid key)
        {
            var entries = db.HashGetAll(key.ToString());
            var type = entries.Single(entry => entry.Name.Equals("Type"));
            var command = getCommand[type.Value.ToString()];
            var instance =(Command) Activator.CreateInstance(command);
            instance.SequenceNumber = Int32.Parse(entries.Single(entry => entry.Name.Equals("SequenceNumber")).Value);
            Switch.On(instance)
                .Case((AddCustomerCommand c) => Restore(c,db,key))
                .Case((AddOrderCommand c)=>Restore(c,db,key))
                .Case((AddProductCommand c)=>Restore(c,db,key))
                .Case((AddProductToOrder c)=>Restore(c,db,key))
                .ElseFail()
                ;
            return instance;
        }

        public static Guid Persist(AddCustomerCommand c, IBatch batch)
        {
            var id = HashCreate(c,batch);
            batch.HashSetAsync(id.ToString(), new []
                {
                    new HashEntry("Id", c.Id),
                    new HashEntry("Version", c.Version),
                    new HashEntry("Firstname", c.Firstname),
                    new HashEntry("Lastname", c.Lastname),
                });
            return id;
        }

        static HashEntry[] GetHash(IDatabase db, Guid key)
        {
            return db.HashGetAll(key.ToString());
        }
       
        public static void Restore(AddCustomerCommand c, IDatabase db, Guid key)
        {
            GetHash(db, key).Tap(hash =>
                {
                    c.Id = hash.Int("Id");
                    c.Version = hash.Int("Version");
                    c.Firstname = hash.String("Firstname");
                    c.Lastname = hash.String("Lastname");
                });
        }

        public static Guid Persist(AddOrderCommand c, IBatch batch)
        {
            var id = HashCreate(c, batch);
            batch.HashSetAsync(id.ToString(), new []
                {
                    new HashEntry("Id", c.Id),
                    new HashEntry("Version", c.Version),
                    new HashEntry("Customer", c.Customer),
                    new HashEntry("OrderDate", c.OrderDate.Ticks),
                });
            return id;
        }
        public static void Restore(AddOrderCommand c,IDatabase db, Guid key)
        {
            GetHash(db, key).Tap(hash =>
                {
                    c.Id = hash.Int("Id");
                    c.Version = hash.Int("Version");
                    c.Customer = hash.Int("Customer");
                    c.OrderDate = new DateTime(hash.Long("OrderDate"));
                });
        }

        public static Guid Persist(AddProductCommand c,IBatch batch)
        {
            var id = HashCreate(c, batch);
            batch.HashSetAsync(id.ToString(), new []
                {
                    new HashEntry("Id", c.Id),
                    new HashEntry("Version", c.Version),
                    new HashEntry("Cost", c.Cost.ToString()),
                    new HashEntry("Name", c.Name),
                });
            return id;
        }
        public static void Restore(AddProductCommand c,IDatabase db, Guid key)
        {
            GetHash(db, key).Tap(hash =>
                {
                    c.Id = hash.Int("Id");
                    c.Version = hash.Int("Version");
                    c.Cost = hash.Float("Cost");
                    c.Name = hash.String("Name");
                });
        }

        public static Guid Persist(AddProductToOrder c, IBatch batch)
        {
            var id = HashCreate(c, batch);
            batch.HashSetAsync(id.ToString(), new []
                {
                    new HashEntry("OrderId", c.OrderId),
                    new HashEntry("ProductId", c.ProductId),
                });
            return id;
        }
        public static void Restore(AddProductToOrder c, IDatabase db, Guid key)
        {
            GetHash(db, key).Tap(hash =>
                {
                    c.OrderId = hash.Int("OrderId");
                    c.ProductId = hash.Int("ProductId");
                });
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