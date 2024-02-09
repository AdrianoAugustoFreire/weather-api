using Dawn;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Exceptions;
using ProductAPI.Model;

namespace ProductAPI.Services
{
	public class ProductRepositoryService : IProductAPIRepositoryService
	{
		private readonly ILogger<ProductRepositoryService> logger;
		private readonly ProductAPIDbContext context;

		public ProductRepositoryService(ProductAPIDbContext context, ILogger<ProductRepositoryService> logger)
		{
			this.context = context;
			this.logger = logger;
		}

		public async Task AddProductAsync(ProductDto newProduct)
		{
			Guard.Argument(() => newProduct).NotNull()
				.Member(f => f.BuyerId, f => f.NotEmpty("BuyerId must not be empty"))
				.Member(f => f.SKU, f => f.NotEmpty("SKU must not be empty"))
				.Member(f => f.Title, f => f.NotEmpty("Title must not be null"));

			var existingProduct = context.Products.Find(newProduct.SKU);
			if (existingProduct != null)
			{
				throw new ProductSKUAlreadyExistsException($"Product with SKU '{newProduct.SKU}' already exists!");
			}

			var productToAdd = new Product
			{
				BuyerId = newProduct.BuyerId!,
				SKU = newProduct.SKU!,
				Title = newProduct.Title!,
				Active = newProduct.Active,
				Description = newProduct.Description
			};

			context.Products.Add(productToAdd);
			await context.SaveChangesAsync();

			logger.LogDebug($"New Product added SKU '{productToAdd.SKU}', Title '{productToAdd.Title}', Descripion '{productToAdd.Description}' Active '{productToAdd.Active}'");
		}

		public async Task DeleteProductAsync(Product productToDelete)
		{
			Guard.Argument(productToDelete, nameof(productToDelete)).NotNull();

			context.Products.Remove(productToDelete);
			await context.SaveChangesAsync();

			logger.LogDebug($"Product deleted. SKU '{productToDelete.SKU}', Title '{productToDelete.Title}', Descripion '{productToDelete.Description}' Active '{productToDelete.Active}'");
		}

		public async Task<List<Product>> GetAllProductsAsync()
		{
			var allProducts = await context.Products.ToListAsync();

			return allProducts;
		}

		public async Task<Product?> GetProductAsync(string sku)
		{
			var product = await context.Products.FirstOrDefaultAsync(a => a.SKU == sku);

			return product;
		}

		public async Task<Product> UpdateProductAsync(ProductDto updatedProduct)
		{
			var productToUpdate = context.Products.First(a => a.SKU == updatedProduct.SKU);

			productToUpdate.Title = updatedProduct.Title!;
			productToUpdate.BuyerId = updatedProduct.BuyerId!;
			productToUpdate.Active = updatedProduct.Active;
			productToUpdate.Description = updatedProduct.Description;

			await context.SaveChangesAsync();

			logger.LogDebug($"Product Updated. SKU '{productToUpdate.SKU}', Title '{productToUpdate.Title}', Descripion '{productToUpdate.Description}' Active '{productToUpdate.Active}'");

			return productToUpdate;
		}
	}
}
