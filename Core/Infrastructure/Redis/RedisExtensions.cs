using System;
using StackExchange.Redis;
using System.Linq;
using SomeBasicFileStoreApp.Core.Commands;
using System.Collections.Generic;
using With;

namespace SomeBasicFileStoreApp.Core.Infrastructure.Redis
{
    public static class RedisExtensions
    {
        public static HashEntry WithName(HashEntry[] hash, string name)
        {
            return hash.Single(h => h.Name == name);
        }
        public static Int32 GetInt(this HashEntry[] hash, string name)
        {
            var v = WithName(hash, name);
            int value;
            if (v.Value.TryParse(out value))
            {
                return value;
            }
            throw new Exception("Failed to parse "+v.Value);
        }
        public static Int64 GetLong(this HashEntry[] hash, string name)
        {
            var v = WithName(hash, name);
            return Int64.Parse(v.Value.ToString());
        }

        public static float GetFloat(this HashEntry[] hash, string name)
        {
            var v = WithName(hash, name);
            return float.Parse(v.Value.ToString());
        }
        public static string GetString(this HashEntry[] hash, string name){
            return WithName(hash, name).Value.ToString();
        }


        public static Guid HashCreate(this Command command, IBatch batch)
        {
            var id = Guid.NewGuid();
            batch.HashSetAsync(id.ToString(), new []
                {
                    new HashEntry("Type", getName[command.GetType()]),
                    new HashEntry("SequenceNumber", command.SequenceNumber)
                });
            
            Switch.On(command)
                .Case((AddCustomerCommand c) => AddCustomerCommandMap.Persist(c, batch, id))
                .Case((AddOrderCommand c)=> AddOrderCommandMap.Persist(c, batch, id))
                .Case((AddProductCommand c)=> AddProductCommandMap.Persist(c, batch, id))
                .Case((AddProductToOrder c)=> AddProductToOrderMap.Persist(c, batch, id))
                .ElseFail()
                ;
            
            return id;
        }
        public static HashEntry[] HashGetAll(this IDatabase db, Guid key)
        {
            return db.HashGetAll(key.ToString());
        }


        public static IDictionary<string,Type> getCommand;
        public static IDictionary<Type, string> getName;
        static RedisExtensions(){
            getCommand = new Dictionary<string,Type>{
                {"AddCustomerCommand", typeof(AddCustomerCommand)},
                {"AddOrderCommand"   , typeof(AddOrderCommand)},
                {"AddProductCommand" , typeof(AddProductCommand)},
                {"AddProductToOrder" , typeof(AddProductToOrder)},
            };
            getName = getCommand.ToDictionary(kv => kv.Value, kv => kv.Key);
        }
    }
}

