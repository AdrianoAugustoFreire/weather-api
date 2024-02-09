using Microsoft.Extensions.Logging;
using Moq;
using ProductAPI.Model;
using ProductAPI.Services;

namespace ProductAPITests
{
	public class ProductActiveValidatorServiceTests
	{
		private readonly ProductActiveValidatorService _productActiveValidatorService;
		private readonly Mock<ILogger<ProductActiveValidatorService>> _mockLogger;
		private readonly Mock<INotify> _mockBuyerNotifyService;

		public ProductActiveValidatorServiceTests()
		{
			_mockLogger = new Mock<ILogger<ProductActiveValidatorService>>();
			_mockBuyerNotifyService = new Mock<INotify>();
			_productActiveValidatorService = new ProductActiveValidatorService(_mockLogger.Object, _mockBuyerNotifyService.Object);
		}

		[Fact]
		public void NotifyOnProductActiveChange_InitialActiveTrue_FinalActiveFalse_CallsNotifyMethod()
		{
			var initialState = new Product { Active = true, SKU = "sku", BuyerId = "buyerId", Title = "Title" };
			var finalState = new Product { Active = false, SKU = "sku", BuyerId = "buyerId", Title = "Title" };

			_productActiveValidatorService.NotifyOnProductActiveChange(initialState, finalState);

			_mockBuyerNotifyService.Verify(x => x.Notify(finalState.BuyerId, It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public void NotifyOnProductActiveChange_InitialActiveFalse_FinalActiveFalse_DoesNotCallNotifyMethod()
		{
			var initialState = new Product { Active = false, SKU = "sku", BuyerId = "buyerId", Title = "Title" };
			var finalState = new Product { Active = false, SKU = "sku", BuyerId = "buyerId", Title = "Title" };

			_productActiveValidatorService.NotifyOnProductActiveChange(initialState, finalState);

			_mockBuyerNotifyService.Verify(x => x.Notify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
		}

		[Fact]
		public void NotifyOnProductActiveChange_InitialActiveTrue_FinalActiveTrue_DoesNotCallNotifyMethod()
		{
			var initialState = new Product { Active = true, SKU = "sku", BuyerId = "buyerId", Title = "Title" };
			var finalState = new Product { Active = true, SKU = "sku", BuyerId = "buyerId", Title = "Title" };

			_productActiveValidatorService.NotifyOnProductActiveChange(initialState, finalState);

			_mockBuyerNotifyService.Verify(x => x.Notify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
		}
	}
}