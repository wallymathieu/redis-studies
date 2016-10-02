using System;
using System.Threading.Tasks;
using SomeBasicFileStoreApp.Core.Commands;
using StackExchange.Redis;
using With;

namespace SomeBasicFileStoreApp.Core.Infrastructure.Redis
{
    public class AddCustomerCommandMap
    {
        public static HashEntry[] ToHash(AddCustomerCommand c)
        {
            return new []
                {
                    new HashEntry("Id", c.Id),
                    new HashEntry("Version", c.Version),
                    new HashEntry("Firstname", c.Firstname),
                    new HashEntry("Lastname", c.Lastname),
                };
        }

        public static void FromHash(AddCustomerCommand c, HashEntry[] hash)
        {
            c.Id = hash.GetInt("Id");
            c.Version = hash.GetInt("Version");
            c.Firstname = hash.GetString("Firstname");
            c.Lastname = hash.GetString("Lastname");
        }
    }
}

