using System;
using System.Threading.Tasks;
using SomeBasicFileStoreApp.Core.Commands;
using StackExchange.Redis;
using With;

namespace SomeBasicFileStoreApp.Core.Infrastructure.Redis
{
    
    public class AddOrderCommandMap
    {
        public static HashEntry[] ToHash(AddOrderCommand c)
        {
            return new []
                {
                    new HashEntry("Id", c.Id),
                    new HashEntry("Version", c.Version),
                    new HashEntry("Customer", c.Customer),
                    new HashEntry("OrderDate", c.OrderDate.Ticks),
                };
        }
        public static void FromHash(AddOrderCommand c,HashEntry[] hash)
        {
            c.Id = hash.GetInt("Id");
            c.Version = hash.GetInt("Version");
            c.Customer = hash.GetInt("Customer");
            c.OrderDate = new DateTime(hash.GetLong("OrderDate"));
        }
    }
}
