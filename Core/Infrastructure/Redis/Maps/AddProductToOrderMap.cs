using System;
using System.Threading.Tasks;
using SomeBasicFileStoreApp.Core.Commands;
using StackExchange.Redis;
using With;

namespace SomeBasicFileStoreApp.Core.Infrastructure.Redis
{
    
    public class AddProductToOrderMap
    {
        public static async Task<Guid> Persist(AddProductToOrder c, IBatch batch, Guid id)
        {
            await batch.HashSetAsync(id.ToString(), new []
                {
                    new HashEntry("OrderId", c.OrderId),
                    new HashEntry("ProductId", c.ProductId),
                });
            return id;
        }
        public static void Restore(AddProductToOrder c, HashEntry[] hash)
        {
            c.OrderId = hash.GetInt("OrderId");
            c.ProductId = hash.GetInt("ProductId");
        }
    }
}
