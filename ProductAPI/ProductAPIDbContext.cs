using Microsoft.EntityFrameworkCore;
using ProductAPI.Model;

namespace ProductAPI
{
	public class ProductAPIDbContext: DbContext
	{
		public virtual DbSet<Product> Products { get; set; }
		public virtual DbSet<Buyer> Buyers { get; set; }

		public string DbPath { get; }

		public ProductAPIDbContext() : base()
		{
			var folder = Environment.SpecialFolder.LocalApplicationData;
			var path = Environment.GetFolderPath(folder);
			DbPath = Path.Join(path, "ProductAPI.sqlite");
		}

		public ProductAPIDbContext(DbContextOptions<ProductAPIDbContext> options): base(options)
		{
			var folder = Environment.SpecialFolder.LocalApplicationData;
			var path = Environment.GetFolderPath(folder);
			DbPath = Path.Join(path, "ProductAPI.sqlite");
		}

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			options.UseSqlite($"Data Source={DbPath}");
		}
	}
}
