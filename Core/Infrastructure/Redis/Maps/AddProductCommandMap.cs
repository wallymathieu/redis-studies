using System;
using System.Threading.Tasks;
using SomeBasicFileStoreApp.Core.Commands;
using StackExchange.Redis;
using With;

namespace SomeBasicFileStoreApp.Core.Infrastructure.Redis
{
    
    public class AddProductCommandMap
    {
        public static async Task<Guid> Persist(AddProductCommand c,IBatch batch, Guid id)
        {
            await batch.HashSetAsync(id.ToString(), new []
                {
                    new HashEntry("Id", c.Id),
                    new HashEntry("Version", c.Version),
                    new HashEntry("Cost", c.Cost.ToString()),
                    new HashEntry("Name", c.Name),
                });
            return id;
        }
        public static void Restore(AddProductCommand c, HashEntry[] hash)
        {
            c.Id = hash.GetInt("Id");
            c.Version = hash.GetInt("Version");
            c.Cost = hash.GetFloat("Cost");
            c.Name = hash.GetString("Name");
        }
    }
}
