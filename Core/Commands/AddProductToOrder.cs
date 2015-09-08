using System.Collections.Generic;
using With;
using With.ReadonlyEnumerable;
using System;
using StackExchange.Redis;

namespace SomeBasicFileStoreApp.Core.Commands
{
	public class AddProductToOrder :Command
	{
		public int OrderId { get; set; }
		public int ProductId { get; set; }

        public override void Handle(IRepository _repository)
        {
            var command = this;
            var order = _repository.GetOrder(command.OrderId);
            var product = _repository.GetProduct(command.ProductId);
            _repository.Save(order.With(o=>
                o.Products.Add(product)));
        }

        public override Guid Persist(IBatch batch)
        {
            var id = this.HashCreate(batch);
            batch.HashSetAsync(id.ToString(), new []
                {
                    new HashEntry("OrderId", OrderId),
                    new HashEntry("ProductId", ProductId),
                });
            return id;
        }
	}
}
