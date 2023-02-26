using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreSystemAPI.Models;
using StoreSystemAPI.Repository;

namespace StoreSystemAPI.Controllers
{
	[ApiController]
	public class StoreController : ControllerBase
	{
		private readonly StoreInterface _storeInterface;
		public StoreController(StoreInterface storeInterface) 
		{
			_storeInterface = storeInterface;
		}
		[HttpGet("api/product")]
		public IEnumerable <Product> GetProducts() 
		{
			return _storeInterface.GetProducts();
		}

		[HttpGet("api/productById")]
		public IEnumerable<Product> GetProductsById(int id)
		{
			return _storeInterface.GetProductById(id);
		}

		[HttpDelete("api/deleteProduct")]
		public bool deleteProduct(int id)
		{
			return _storeInterface.DeleteProduct(id);
		}
		[HttpPost("api/addProduct")]
		public bool addProduct(Product product)
		{
			return _storeInterface.AddProduct(product);
		}

		[HttpGet("api/getProductNames")]
		public ActionResult<IEnumerable<string>> GetProductNamesByIds(string ids)
		{
			string[] idArray = ids.Split(',');

			var productNames = _storeInterface.GetProductNamesByIds(idArray);

			return productNames.ToArray();
		}

		[HttpPut("api/updateProduct")]
		public bool UpdateProduct(int id,[FromBody] Product productData)
		{
			return _storeInterface.UpdateProduct(id, productData);
		}
	}
}
