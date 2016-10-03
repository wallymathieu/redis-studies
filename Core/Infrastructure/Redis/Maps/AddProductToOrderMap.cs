using SomeBasicFileStoreApp.Core.Commands;
using StackExchange.Redis;

namespace SomeBasicFileStoreApp.Core.Infrastructure.Redis
{

    public class AddProductToOrderMap
    {
        public static HashEntry[] ToHash(AddProductToOrder c)
        {
            return new[]
                {
                    new HashEntry("OrderId", c.OrderId),
                    new HashEntry("ProductId", c.ProductId),
                };
        }
        public static void FromHash(AddProductToOrder c, HashEntry[] hash)
        {
            c.OrderId = hash.GetInt("OrderId");
            c.ProductId = hash.GetInt("ProductId");
        }
    }
}
