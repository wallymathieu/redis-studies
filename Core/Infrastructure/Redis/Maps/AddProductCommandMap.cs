using System;
using System.Threading.Tasks;
using SomeBasicFileStoreApp.Core.Commands;
using StackExchange.Redis;
using With;

namespace SomeBasicFileStoreApp.Core.Infrastructure.Redis
{
    
    public class AddProductCommandMap
    {
        public static HashEntry[] ToHash(AddProductCommand c)
        {
            return new[]
                {
                    new HashEntry("Id", c.Id),
                    new HashEntry("Version", c.Version),
                    new HashEntry("Cost", c.Cost.ToString()),
                    new HashEntry("Name", c.Name),
            };
        }
        public static void FromHash(AddProductCommand c, HashEntry[] hash)
        {
            c.Id = hash.GetInt("Id");
            c.Version = hash.GetInt("Version");
            c.Cost = hash.GetFloat("Cost");
            c.Name = hash.GetString("Name");
        }
    }
}
