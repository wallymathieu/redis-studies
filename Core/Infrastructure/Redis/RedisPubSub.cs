using System;
using SomeBasicFileStoreApp.Core.Commands;
using StackExchange.Redis;

namespace SomeBasicFileStoreApp.Core
{
    public class RedisPubSub
    {
        readonly IDatabase db;

        public RedisPubSub(IDatabase db)
        {
            this.db = db;
        }
        public void Publish(Guid[] ids)
        {
            db.Publish("CommandsAdded", Command.ToString(ids));
        }
        public void Start(Action<Guid[]> onEvent) 
        {
            db.Multiplexer.GetSubscriber().Subscribe("CommandsAdded", (channel, value) =>
             {
                onEvent(Command.ParseUniqueIds(value));
             });
        }
    }
}
