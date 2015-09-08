using System;
using StackExchange.Redis;
namespace SomeBasicFileStoreApp.Core.Commands
{
    public class AddOrderCommand : Command
    {
        public virtual int Id { get; private set; }
        public virtual int Version { get; private set; }
        public virtual int Customer { get; private set; }
        public virtual DateTime OrderDate { get; private set; }
       
        public override void Handle(IRepository _repository)
        {
            var command = this;
            _repository.Save(new Order(command.Id, command.Customer, command.OrderDate, new Product[0], command.Version));
        }

        public override Guid Persist(IBatch batch)
        {
            var id = this.HashCreate(batch);
            batch.HashSetAsync(id.ToString(), new []
                {
                    new HashEntry("Id", Id),
                    new HashEntry("Version", Version),
                    new HashEntry("Customer", Customer),
                    new HashEntry("OrderDate", OrderDate.Ticks),
                });
            return id;
        }
    }
}
