using System;
using StackExchange.Redis;
using System.Linq;

namespace SomeBasicFileStoreApp.Core
{
    public static class RedisExtensions
    {
        public static HashEntry WithName(this HashEntry[] hash, string name)
        {
            return hash.Single(h => h.Name == name);
        }
        public static Int32 Int(this HashEntry[] hash, string name)
        {
            var v = WithName(hash, name);
            int value;
            if (v.Value.TryParse(out value))
            {
                return value;
            }
            throw new Exception("Failed to parse "+v.Value);
        }
        public static Int64 Long(this HashEntry[] hash, string name)
        {
            var v = WithName(hash, name);
            return Int64.Parse(v.Value.ToString());
        }

        public static float Float(this HashEntry[] hash, string name)
        {
            var v = WithName(hash, name);
            return float.Parse(v.Value.ToString());
        }
        public static string String(this HashEntry[] hash, string name){
            return WithName(hash, name).Value.ToString();
        }

    }
}

