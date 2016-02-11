using With;
using With.ReadonlyEnumerable;

namespace SomeBasicFileStoreApp.Core.Commands
{
    public class AddProductToOrder : Command
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        public override void Handle(IRepository _repository)
        {
            var command = this;
            var order = _repository.GetOrder(command.OrderId);
            var product = _repository.GetProduct(command.ProductId);
            _repository.Save(order.With(o =>
                o.Products.Add(product)));
        }


    }
}
