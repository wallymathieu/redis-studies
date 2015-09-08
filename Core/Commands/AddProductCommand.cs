using StackExchange.Redis;
using System;
namespace SomeBasicFileStoreApp.Core.Commands
{
    public class AddProductCommand : Command
    {
        public virtual int Id { get; private set; }
        public virtual int Version { get; private set; }
        public virtual float Cost { get; private set; }
        public virtual string Name { get; private set; }
    
        public override void Handle(IRepository _repository)
        {
            var command = this;
            _repository.Save(new Product(command.Id, command.Cost, command.Name, command.Version));
        }

        public override Guid Persist(IBatch batch)
        {
            var id = this.HashCreate(batch);
            batch.HashSetAsync(id.ToString(), new []
                {
                    new HashEntry("Id", Id),
                    new HashEntry("Version", Version),
                    new HashEntry("Cost", Cost),
                    new HashEntry("Name", Name),
                });
            return id;
        }
    }
}
