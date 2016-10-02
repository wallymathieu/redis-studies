using System;
using System.Threading.Tasks;
using SomeBasicFileStoreApp.Core.Commands;
using StackExchange.Redis;
using With;

namespace SomeBasicFileStoreApp.Core.Infrastructure.Redis
{
    public class AddCustomerCommandMap
    {
        public static async Task<Guid> Persist(AddCustomerCommand c, IBatch batch, Guid id)
        {
            await batch.HashSetAsync(id.ToString(), new []
                {
                    new HashEntry("Id", c.Id),
                    new HashEntry("Version", c.Version),
                    new HashEntry("Firstname", c.Firstname),
                    new HashEntry("Lastname", c.Lastname),
                });
            return id;
        }

        public static void Restore(AddCustomerCommand c, HashEntry[] hash)
        {
            c.Id = hash.GetInt("Id");
            c.Version = hash.GetInt("Version");
            c.Firstname = hash.GetString("Firstname");
            c.Lastname = hash.GetString("Lastname");
        }
    }
}

