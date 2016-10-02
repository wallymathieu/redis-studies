using System;
using System.Linq;
using With;
using With.ReadonlyEnumerable;

namespace SomeBasicFileStoreApp.Core.Commands
{
    public class AddProductToOrder : Command
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public AddProductToOrder()
        {
        }
        public AddProductToOrder(Guid id) : base(id)
        {
        }

        public override void Handle(IRepository repository)
        {
            var command = this;
            var order = repository.GetOrder(command.OrderId);
            var product = repository.GetProduct(command.ProductId);
            if (!order.Products.Any(p=>p.Id.Equals(command.ProductId)))
                repository.Save(order.With(o =>
                    o.Products.Add(product)));
        }
    }
}
