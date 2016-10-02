using System.Collections.Generic;

namespace SomeBasicFileStoreApp.Core
{
	public interface IRepository
	{
		Customer GetCustomer(int v);
        Order GetOrder(int v);
		Product GetProduct(int v);
        bool TryGetCustomer(int v, out Customer customer);
        bool TryGetOrder(int v, out Order order);
        bool TryGetProduct(int v, out Product product);

		IEnumerable<Customer> QueryOverCustomers();
		IEnumerable<Product> QueryOverProducts();
		void Save(Product obj);
		void Save(Order obj);
		void Save(Customer obj);
		Customer GetTheCustomerOrder(int v);
	}
}