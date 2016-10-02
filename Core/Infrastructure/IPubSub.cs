using System;
namespace SomeBasicFileStoreApp.Core.Infrastructure
{
    public interface IPubSub
    {
        void Publish(Guid[] ids);
        void Start(Action<Guid[]> onEvent);
    }
}
