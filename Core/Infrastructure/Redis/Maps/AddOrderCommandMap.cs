using System;
using System.Threading.Tasks;
using SomeBasicFileStoreApp.Core.Commands;
using StackExchange.Redis;
using With;

namespace SomeBasicFileStoreApp.Core.Infrastructure.Redis
{
    
    public class AddOrderCommandMap
    {
        public static async Task<Guid> Persist(AddOrderCommand c, IBatch batch, Guid id)
        {
            await batch.HashSetAsync(id.ToString(), new []
                {
                    new HashEntry("Id", c.Id),
                    new HashEntry("Version", c.Version),
                    new HashEntry("Customer", c.Customer),
                    new HashEntry("OrderDate", c.OrderDate.Ticks),
                });
            return id;
        }
        public static void Restore(AddOrderCommand c,HashEntry[] hash)
        {
            c.Id = hash.GetInt("Id");
            c.Version = hash.GetInt("Version");
            c.Customer = hash.GetInt("Customer");
            c.OrderDate = new DateTime(hash.GetLong("OrderDate"));
        }
    }
}
