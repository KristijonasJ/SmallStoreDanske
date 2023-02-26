using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using StoreSystemAPI.Models;
using StoreSystemAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assert = NUnit.Framework.Assert;

namespace StoreSystemTest
{
	public class PublishersControllerTests
	{
		private Methods _methods;

		private static DbContextOptions<StoreDbContext> dbContextOptions = new DbContextOptionsBuilder<StoreDbContext>()
			.UseInMemoryDatabase(databaseName: "TestDatabase")
			.Options;

		StoreDbContext context;


		[SetUp]
		public void Setup()
		{
			context = new StoreDbContext(dbContextOptions);
			_methods = new Methods(context);

			context.Database.EnsureCreated();

			SeedDatabase();

		}


		private void SeedDatabase()
		{
			var products = new List<Product>()
			{
				new Product()
				{
					Id = 1, Name = "Product 1", Type = "Type 1", Price = 10.0, Quantity = 5
				},
				new Product()
				{
					Id = 2, Name = "Product 2", Type = "Type 2", Price = 20.0, Quantity = 10
				},
				new Product()
				{
					Id = 3, Name = "Product 3", Type = "Type 3", Price = 30.0, Quantity = 15
				},
			};
			context.Products.AddRange(products);
			context.SaveChanges();
		}

		[Fact]
		public void GetProducts_ReturnsListOfProducts()
		{
			Setup();

			var result = _methods.GetProducts();

			Assert.IsInstanceOf<IEnumerable<Product>>(result);
			Assert.AreEqual(3, result.Count());
		}

		[Fact]
		public void GetProductById_Returns_Product_With_Given_Id()
		{
			Setup();
			var id = 2;

			var result = _methods.GetProductById(id);

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count());

			var product = result.First();
			Assert.AreEqual(id, product.Id);
			Assert.AreEqual("Product 2", product.Name);
			Assert.AreEqual("Type 2", product.Type);
			Assert.AreEqual(20.0, product.Price);
			Assert.AreEqual(10, product.Quantity);
		}

		[Fact]
		public void GetProductById_Returns_Empty_List_When_Id_Does_Not_Exist()
		{
			Setup();
			var id = 999;
			var result = _methods.GetProductById(id);

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count());
		}
		[Fact]
		public void DeleteProduct_Deletes_Product_With_Given_Id()
		{
			Setup();
			var id = 2;
			var result = _methods.DeleteProduct(id);

			Assert.IsTrue(result);

			var product = context.Products.FirstOrDefault(x => x.Id == id);
			Assert.IsNull(product);
		}
		[Fact]
		public void DeleteProduct_Returns_False_When_Product_With_Given_Id_Does_Not_Exist()
		{
			Setup();

			var id = 999;
			var result = _methods.DeleteProduct(id);
			Assert.IsFalse(result);
		}

		[Fact]
		public void AddProduct_Adds_Product_When_Valid_Product_Is_Provided()
		{
			Setup();
			var product = new Product()
			{
				Id = 4,
				Name = "Product 4",
				Type = "Type 4",
				Price = 40.0,
				Quantity = 20
			};

			var result = _methods.AddProduct(product);
			Assert.IsTrue(result);

			var addedProduct = context.Products.FirstOrDefault(p => p.Id == product.Id);
			Assert.IsNotNull(addedProduct);
			Assert.AreEqual(product.Name, addedProduct.Name);
			Assert.AreEqual(product.Type, addedProduct.Type);
			Assert.AreEqual(product.Price, addedProduct.Price);
			Assert.AreEqual(product.Quantity, addedProduct.Quantity);
		}

		[Fact]
		public void AddProduct_Returns_False_When_Product_With_Same_Name_And_Type_Already_Exists()
		{
			Setup();
			var existingProduct = new Product()
			{
				Id = 5,
				Name = "Product 5",
				Type = "Type 5",
				Price = 50.0,
				Quantity = 25
			};
			context.Products.Add(existingProduct);
			context.SaveChanges();

			var product = new Product()
			{
				Id = 6,
				Name = existingProduct.Name,
				Type = existingProduct.Type,
				Price = 60.0,
				Quantity = 30
			};

			var result = _methods.AddProduct(product);

			Assert.IsFalse(result);

			var addedProduct = context.Products.FirstOrDefault(p => p.Id == product.Id);
			Assert.IsNull(addedProduct);
		}
		[Fact]
		public void AddProduct_Returns_False_When_Product_Is_Null()
		{
			Setup();
			Product product = null;

			var result = _methods.AddProduct(product);
			Assert.IsFalse(result);
		}

		[Fact]
		public void AddProduct_Returns_False_When_Product_Name_Is_Empty()
		{
			Setup();
			var product = new Product()
			{
				Id = 7,
				Name = "",
				Type = "Type 7",
				Price = 70.0,
				Quantity = 35
			};

			var result = _methods.AddProduct(product);

			Assert.IsFalse(result);

			var addedProduct = context.Products.FirstOrDefault(p => p.Id == product.Id);
			Assert.IsNull(addedProduct);
		}

		[Fact]
		public void UpdateProduct_Returns_False_When_ProductId_Does_Not_Exist()
		{
			Setup();

			var productId = 999;
			var productData = new Product { Name = "Updated Product", Type = "Updated Type", Price = 50.0, Quantity = 20 };

			var result = _methods.UpdateProduct(productId, productData);

			Assert.IsFalse(result);
		}

		[Fact]
		public void UpdateProduct_Returns_False_When_ProductData_Is_Null()
		{
			Setup();

			var productId = 1;
			Product productData = null;

			var result = _methods.UpdateProduct(productId, productData);
			Assert.IsFalse(result);
		}
		[Fact]
		public void UpdateProduct_Returns_False_When_ProductData_Is_Not_Valid()
		{
			Setup();

			var productId = 1;
			var productData = new Product { Name = "", Type = "", Price = -1.0, Quantity = -1 };

			var result = _methods.UpdateProduct(productId, productData);

			Assert.IsFalse(result);
		}

		[Fact]
		public void UpdateProduct_Returns_False_When_ProductData_Is_Same_As_Existing_Product()
		{
			Setup();

			var productId = 1;
			var productData = new Product { Name = "Product 1", Type = "Type 1", Price = 10.0, Quantity = 5 };

			var result = _methods.UpdateProduct(productId, productData);

			Assert.IsFalse(result);
		}

		[Fact]
		public void UpdateProduct_Updates_ProductData_For_Existing_Product()
		{
			Setup();

			var productId = 1;
			var productData = new Product { Name = "Updated Product", Type = "Updated Type", Price = 50.0, Quantity = 20 };

			var result = _methods.UpdateProduct(productId, productData);

			Assert.IsTrue(result);

			var updatedProduct = context.Products.FirstOrDefault(x => x.Id == productId);
			Assert.IsNotNull(updatedProduct);
			Assert.AreEqual(productData.Name, updatedProduct.Name);
			Assert.AreEqual(productData.Type, updatedProduct.Type);
			Assert.AreEqual(productData.Price, updatedProduct.Price);
			Assert.AreEqual(productData.Quantity, updatedProduct.Quantity);
		}

	}
}