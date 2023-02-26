using Newtonsoft.Json;
using StoreSystemAPI.Models;
using System.Text.Json;

namespace StoreSystemAPI.Repository
{
	public class Methods : StoreInterface
	{
		public StoreDbContext _storeContext { get;}

		public Methods(StoreDbContext storeContext)
		{
			_storeContext= storeContext;
		}

		public IEnumerable <Product> GetProducts()
		{
			List<Product> products = new List<Product>();
			try
			{
				products = _storeContext.Products.ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return products;
		}

		public IEnumerable<Product> GetProductById(int id)
		{
			List<Product> products = new List<Product>();
			try
			{
				products = _storeContext.Products.Where(x => x.Id == id).ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return products;
		}

		public bool DeleteProduct(int id)
		{
			try
			{
				var product = _storeContext.Products.Where(x => x.Id == id).FirstOrDefault();
				if (product != null)
				{
					_storeContext.Products.Remove(product);
					_storeContext.SaveChanges();
				}
				else
				{
					return false;
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;

			}
			return true;
		}

		public bool AddProduct(Product product)
		{
			try
			{	
				if(product != null && product.Name != "" && product.Type != "")
				{
					var checkProduct = _storeContext.Products.Where(x => x.Name == product.Name && x.Type == product.Type).FirstOrDefault();
					if(checkProduct == null)
					{
						_storeContext.Products.Add(product);
						_storeContext.SaveChanges();
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
			return true;
		}

		public IEnumerable<string> GetProductNamesByIds(string[] ids)
		{
			List<string> productNames = new List<string>();
			try
			{
				foreach (var id in ids)
				{
					var product = _storeContext.Products.Where(x => x.Id == int.Parse(id)).FirstOrDefault();
					if (product != null)
					{
						productNames.Add(product.Name);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return productNames;
		}
		
		public bool UpdateProduct(int productId, Product productData)
		{
			try
			{
				var product = _storeContext.Products.Where(x => x.Id == productId).FirstOrDefault();
				if(productData!= null && product != null && productData.Quantity > 0 && productData.Price > 0)
				{
					if(productData.Name != product.Name || productData.Quantity != product.Quantity || productData.Price != product.Price || productData.Type != product.Type)
					{
						product.Name = productData.Name;
						product.Quantity = productData.Quantity;
						product.Price = productData.Price;
						product.Type = productData.Type;
						_storeContext.Products.Update(product);
						_storeContext.SaveChanges();
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
			return true;

		}
	}
}
