using System;
using StackExchange.Redis;
namespace SomeBasicFileStoreApp.Core.Commands
{
    public class AddOrderCommand : Command
    {
        public virtual int Id { get;  set; }
        public virtual int Version { get;  set; }
        public virtual int Customer { get;  set; }
        public virtual DateTime OrderDate { get;  set; }
       
        public override void Handle(IRepository _repository)
        {
            var command = this;
            _repository.Save(new Order(command.Id, command.Customer, command.OrderDate, new Product[0], command.Version));
        }


    }
}
