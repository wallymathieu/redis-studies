using System;
using SomeBasicFileStoreApp.Core.Commands;
using StackExchange.Redis;
using With;

namespace SomeBasicFileStoreApp.Core.Infrastructure.Redis
{
    
    public class AddProductToOrderMap
    {
        public static Guid Persist(AddProductToOrder c, IBatch batch, Guid id)
        {
            batch.HashSetAsync(id.ToString(), new []
                {
                    new HashEntry("OrderId", c.OrderId),
                    new HashEntry("ProductId", c.ProductId),
                });
            return id;
        }
        public static void Restore(AddProductToOrder c, IDatabase db, Guid key)
        {
            db.HashGetAll(key).Tap(hash =>
                {
                    c.OrderId = hash.GetInt("OrderId");
                    c.ProductId = hash.GetInt("ProductId");
                });
        }
    }
}
