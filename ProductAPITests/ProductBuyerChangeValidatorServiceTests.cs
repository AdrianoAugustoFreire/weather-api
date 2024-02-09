using Microsoft.Extensions.Logging;
using Moq;
using ProductAPI.Model;
using ProductAPI.Services;

namespace ProductAPITests
{
	public class ProductBuyerChangeValidatorServiceTests
	{
		private readonly ProductBuyerChangeValidatorService _productBuyerChangeValidatorService;
		private readonly Mock<ILogger<ProductBuyerChangeValidatorService>> _mockLogger;
		private readonly Mock<INotify> _mockBuyerNotifyService;

		public ProductBuyerChangeValidatorServiceTests()
		{
			_mockLogger = new Mock<ILogger<ProductBuyerChangeValidatorService>>();
			_mockBuyerNotifyService = new Mock<INotify>();
			_productBuyerChangeValidatorService = new ProductBuyerChangeValidatorService(_mockLogger.Object, _mockBuyerNotifyService.Object);
		}

		[Fact]
		public void NotifyOnProductBuyerChange_BuyerIdChanged_CallsNotifyMethodForBothNewAndPreviousBuyer()
		{
			var initialState = new Product { BuyerId = "buyerId1", SKU = "sku", Title = "Title" };
			var finalState = new Product { BuyerId = "buyerId2", SKU = "sku", Title = "Title" };

			_productBuyerChangeValidatorService.NotifyOnProductBuyerChange(initialState, finalState);

			_mockBuyerNotifyService.Verify(x => x.Notify(finalState.BuyerId, It.IsAny<string>()), Times.Once);
			_mockBuyerNotifyService.Verify(x => x.Notify(initialState.BuyerId, It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public void NotifyOnProductBuyerChange_BuyerIdNotChanged_DoesNotCallNotifyMethod()
		{
			var initialState = new Product { BuyerId = "buyerId", SKU = "sku", Title = "Title" };
			var finalState = new Product { BuyerId = "buyerId", SKU = "sku", Title = "Title" };

			_productBuyerChangeValidatorService.NotifyOnProductBuyerChange(initialState, finalState);

			_mockBuyerNotifyService.Verify(x => x.Notify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
		}
	}
}
