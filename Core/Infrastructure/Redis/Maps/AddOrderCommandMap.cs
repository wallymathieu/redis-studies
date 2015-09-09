using System;
using SomeBasicFileStoreApp.Core.Commands;
using StackExchange.Redis;
using With;

namespace SomeBasicFileStoreApp.Core.Infrastructure.Redis
{
    
    public class AddOrderCommandMap
    {
        public static Guid Persist(AddOrderCommand c, IBatch batch, Guid id)
        {
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
            db.HashGetAll(key).Tap(hash =>
                {
                    c.Id = hash.GetInt("Id");
                    c.Version = hash.GetInt("Version");
                    c.Customer = hash.GetInt("Customer");
                    c.OrderDate = new DateTime(hash.GetLong("OrderDate"));
                });
        }
    }
}
