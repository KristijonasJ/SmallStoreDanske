using Microsoft.EntityFrameworkCore;
using StoreSystemAPI.Models;

namespace StoreSystemAPI.Repository
{
	public class StoreDbContext : DbContext
	{
		public StoreDbContext(DbContextOptions<StoreDbContext> options)
		: base(options)
		{
		}
		public virtual DbSet<Product> Products { get; set; }
	}
}
