using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeBasicFileStoreApp.Core.Commands
{
	public abstract class Command
	{
        private static IDictionary<string,Type> getCommand;
        private static IDictionary<Type, string> getName;
        static Command(){
            getCommand = new Dictionary<string,Type>{
                {"AddCustomerCommand", typeof(AddCustomerCommand)},
                {"AddOrderCommand"   , typeof(AddOrderCommand)},
                {"AddProductCommand" , typeof(AddProductCommand)},
                {"AddProductToOrder" , typeof(AddProductToOrder)},
            };
            getName = getCommand.ToDictionary(kv => kv.Value, kv => kv.Key);
        }

        public long SequenceNumber { get; set; }
        public abstract void Handle(IRepository repository);
        public abstract Guid Persist(IBatch batch);
        protected Guid HashCreate(IBatch batch){
            var id = Guid.NewGuid();
            batch.HashSetAsync(id.ToString(), new []
                {
                    new HashEntry("Type", getName[this.GetType()]),
                    new HashEntry("SequenceNumber", SequenceNumber)
                });
            return id;
        }

        public Command Read(IDatabase db, Guid key)
        {
            var entries = db.HashGetAll(key.ToString());
            var type = entries.Single(entry => entry.Name.Equals("Type"));
            var command = getCommand[type.Value.ToString()];
            throw new NotImplementedException();
        }
	}
}