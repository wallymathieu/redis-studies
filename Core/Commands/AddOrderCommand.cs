using System;
namespace SomeBasicFileStoreApp.Core.Commands
{
    public class AddOrderCommand : Command
    {
        public virtual int Id { get;  set; }
        public virtual int Version { get;  set; }
        public virtual int Customer { get;  set; }
        public virtual DateTime OrderDate { get;  set; }
        public AddOrderCommand()
        {
        }
        public AddOrderCommand(Guid id) : base(id)
        {
        }
        public override void Handle(Repository repository)
        {
            var command = this;
            Order o;
            if (!repository.TryGetOrder(command.Id, out o))
            {
                repository.Save(new Order(command.Id, command.Customer, command.OrderDate, new Product[0], command.Version));
            }
        }
    }
}
