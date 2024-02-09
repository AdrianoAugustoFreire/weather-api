using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using ProductAPI;
using ProductAPI.Controllers;
using ProductAPI.Model;
using ProductAPI.Services;

namespace ProductAPITests
{
	public class ProductAPIControllerTests
	{

		private readonly ProductAPIController _sut;
		private readonly Mock<IProductAPIRepositoryService> _mockRepositoryService;
		private readonly Mock<ILogger<ProductAPIController>> _mockLogger;

		public ProductAPIControllerTests()
		{
			_mockRepositoryService = new Mock<IProductAPIRepositoryService>();
			_mockLogger = new Mock<ILogger<ProductAPIController>>();
			_sut = new ProductAPIController(_mockLogger.Object, _mockRepositoryService.Object);
		}

		private List<Buyer> GetSampleBuyers()
		{
			return new List<Buyer>
			{
				new Buyer
				{
					Id = "49ec2a8703224eea9dec16b22546477e",
					Name = "Johnny Buyer",
					Email = "jbuyer@test.com"
				},
				new Buyer
				{
					Id = "a790a7b6bf2a48569066c46306c3332d",
					Name = "Jennie Purchaser",
					Email = "jpurchaser@test.com"
				}
			};
		}

		private List<Product> GetSampleProducts()
		{
			return new List<Product>
			{
				new Product
				{
					Active = true,
					BuyerId = "49ec2a8703224eea9dec16b22546477e",
					Description = "A very nice Sofa",
					SKU = "fd2e0f8f-d31d-4a48-ba1f-aa17b1b8b9a2",
					Title = "Sofa"
				},
				new Product
				{
					Active = true,
					BuyerId = "a790a7b6bf2a48569066c46306c3332d",
					Description = "A red colored bike",
					SKU = "5b8d1b16-33f8-4e16-98a1-7a3f1f88e4b5",
					Title = "Bike"
				},
			};
		}

		[Fact]
		public async Task Put_WhenProductIsNull_ReturnsBadRequest()
		{
			ProductDto? product = null;

			IActionResult result = await _sut.Put(product!);

			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task Put_WhenProductSKUIsNullOrEmpty_ReturnsBadRequest()
		{
			var updatedProduct = new ProductDto { SKU = null };

			IActionResult result = await _sut.Put(updatedProduct);

			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task Put_WhenExistingProductIsNull_ReturnsNotFound()
		{
			var updatedProduct = new ProductDto { SKU = "some_sku" };

			_mockRepositoryService.Setup(repo => repo.GetProductAsync(updatedProduct.SKU)).ReturnsAsync((Product)null);

			IActionResult result = await _sut.Put(updatedProduct);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task AddProduct_ValidData_AddsToDatabase()
		{
			var loggerMock = new Mock<ILogger<ProductRepositoryService>>();

			var products = GetSampleProducts();

			ProductDto product = new()
			{
				Active = true,
				BuyerId = "a790a7b6bf2a48569066c46306c3332d",
				Description = "A blue colored bike",
				SKU = "5b8d1b16-3333-4e16-98a1-7a3f1f88e4b5",
				Title = "Bike"
			};

			var dbContextMock = new Mock<ProductAPIDbContext>();

			dbContextMock.Setup(x => x.Products).ReturnsDbSet(products);

			var service = new ProductRepositoryService(dbContextMock.Object, loggerMock.Object);

			await service.AddProductAsync(product);

			dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task Delete_ProductExists_ReturnsNoContent()
		{
			var productSkuToDelete = "fd2e0f8f-d31d-4a48-ba1f-aa17b1b8b9a2";
			var productToDelete = new Product { SKU = productSkuToDelete, Title = "Title", BuyerId = "12345" };

			_mockRepositoryService.Setup(repo => repo.GetProductAsync(productSkuToDelete)).ReturnsAsync(productToDelete);
			_mockRepositoryService.Setup(repo => repo.DeleteProductAsync(productToDelete)).Returns(Task.CompletedTask);

			var sut = new ProductAPIController(_mockLogger.Object, _mockRepositoryService.Object);

			IActionResult result = await sut.Delete(productSkuToDelete);

			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task Delete_ProductDoesNotExist_ReturnsNotFound()
		{
			var productSkuToDelete = "fd2e0f8f-d31d-4a48-ba1f-aa17b1b8b9a2";
			var productToDelete = new Product { SKU = productSkuToDelete, Title = "Title", BuyerId = "12345" };

			_mockRepositoryService.Setup(repo => repo.GetProductAsync(productSkuToDelete)).ReturnsAsync((Product)null);

			var sut = new ProductAPIController(_mockLogger.Object, _mockRepositoryService.Object);

			IActionResult result = await sut.Delete(productSkuToDelete);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task Get_ProductExists_ReturnsOkResultWithProduct()
		{
			string sku = "existing_sku";
			Product mockProduct = new Product { SKU = sku, Title = "Title", BuyerId = "12345" };

			_mockRepositoryService.Setup(repo => repo.GetProductAsync(sku)).ReturnsAsync(mockProduct);

			var sut = new ProductAPIController(_mockLogger.Object, _mockRepositoryService.Object);

			ActionResult result = await sut.Get(sku);

			Assert.IsType<OkObjectResult>(result);

			var okResult = result as OkObjectResult;
			Assert.Equal(mockProduct, okResult.Value);
		}

		[Fact]
		public async Task Get_ProductDoesNotExist_ReturnsNotFoundResult()
		{
			string sku = "non_existing_sku";
			_mockRepositoryService.Setup(repo => repo.GetProductAsync(sku)).ReturnsAsync((Product)null);

			var sut = new ProductAPIController(_mockLogger.Object, _mockRepositoryService.Object);

			ActionResult result = await sut.Get(sku);

			Assert.IsType<NotFoundResult>(result);
		}
	}
}

