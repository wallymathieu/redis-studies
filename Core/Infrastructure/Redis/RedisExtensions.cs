using System;
using StackExchange.Redis;
using System.Linq;
using SomeBasicFileStoreApp.Core.Commands;
using System.Collections.Generic;
using With;
using System.Threading.Tasks;

namespace SomeBasicFileStoreApp.Core.Infrastructure.Redis
{
    public static class RedisExtensions
    {
        public static HashEntry WithName(HashEntry[] hash, string name)
        {
            return hash.Single(h => h.Name == name);
        }
        public static int GetInt(this HashEntry[] hash, string name)
        {
            var v = WithName(hash, name);
            int value;
            if (v.Value.TryParse(out value))
            {
                return value;
            }
            throw new Exception("Failed to parse "+v.Value);
        }
        public static long GetLong(this HashEntry[] hash, string name)
        {
            var v = WithName(hash, name);
            return long.Parse(v.Value.ToString());
        }

        public static float GetFloat(this HashEntry[] hash, string name)
        {
            var v = WithName(hash, name);
            return float.Parse(v.Value.ToString());
        }
        public static string GetString(this HashEntry[] hash, string name){
            return WithName(hash, name).Value.ToString();
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

