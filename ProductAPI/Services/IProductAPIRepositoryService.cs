using ProductAPI.Model;

namespace ProductAPI.Services
{
	public interface IProductAPIRepositoryService
	{
		Task AddProductAsync(ProductDto newProduct);
		Task<Product> UpdateProductAsync(ProductDto updatedProduct);
		Task DeleteProductAsync(Product productToDelete);
		Task<Product?> GetProductAsync(string sku);
		Task<List<Product>> GetAllProductsAsync();
	}
}