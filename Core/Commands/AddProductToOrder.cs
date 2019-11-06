using System;
using System.Collections.Generic;
using System.Linq;
using With;

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
        private static readonly IPreparedCopy<Order, IEnumerable<Product>> ProductsCopy =
            Prepare.Copy<Order, IEnumerable<Product>>((o, v) => o.Products == v);
        public override void Handle(Repository repository)
        {
            var command = this;
            var order = repository.GetOrder(command.OrderId);
            var product = repository.GetProduct(command.ProductId);
            if (!order.Products.Any(p => p.Id.Equals(command.ProductId)))
            {
                repository.Save(ProductsCopy.Copy(order, new List<Product>(order.Products) { product }));
            }

        }
    }
}
