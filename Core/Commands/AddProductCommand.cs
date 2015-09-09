using StackExchange.Redis;
using System;
namespace SomeBasicFileStoreApp.Core.Commands
{
    public class AddProductCommand : Command
    {
        public virtual int Id { get;  set; }
        public virtual int Version { get;  set; }
        public virtual float Cost { get;  set; }
        public virtual string Name { get;  set; }
    
        public override void Handle(IRepository _repository)
        {
            var command = this;
            _repository.Save(new Product(command.Id, command.Cost, command.Name, command.Version));
        }
    }
}
