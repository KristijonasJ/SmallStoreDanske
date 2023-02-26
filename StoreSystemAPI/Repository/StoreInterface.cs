using Microsoft.AspNetCore.Mvc;
using StoreSystemAPI.Models;

namespace StoreSystemAPI.Repository
{
	public interface StoreInterface
	{
		public IEnumerable<Product>GetProducts();
		public IEnumerable<Product> GetProductById(int id);
		public bool DeleteProduct(int id);
		public bool AddProduct(Product product);
		IEnumerable<string> GetProductNamesByIds(string[] ids);
		public bool UpdateProduct(int productId, Product productData);

	}
}
