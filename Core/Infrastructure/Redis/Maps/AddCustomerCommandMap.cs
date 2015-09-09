using System;
using SomeBasicFileStoreApp.Core.Commands;
using StackExchange.Redis;
using With;

namespace SomeBasicFileStoreApp.Core.Infrastructure.Redis
{
    public class AddCustomerCommandMap
    {
        public static Guid Persist(AddCustomerCommand c, IBatch batch, Guid id)
        {
            batch.HashSetAsync(id.ToString(), new []
                {
                    new HashEntry("Id", c.Id),
                    new HashEntry("Version", c.Version),
                    new HashEntry("Firstname", c.Firstname),
                    new HashEntry("Lastname", c.Lastname),
                });
            return id;
        }

        public static void Restore(AddCustomerCommand c, IDatabase db, Guid key)
        {
            db.HashGetAll(key).Tap(hash =>
                {
                    c.Id = hash.GetInt("Id");
                    c.Version = hash.GetInt("Version");
                    c.Firstname = hash.GetString("Firstname");
                    c.Lastname = hash.GetString("Lastname");
                });
        }
    }
}

