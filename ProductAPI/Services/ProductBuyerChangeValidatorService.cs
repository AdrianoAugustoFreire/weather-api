using ProductAPI.Model;

namespace ProductAPI.Services
{
	public class ProductBuyerChangeValidatorService : IProductBuyerChangeValidatorService
	{
		private readonly ILogger<ProductBuyerChangeValidatorService> logger;
		private readonly INotify buyerNotifyService;

		public ProductBuyerChangeValidatorService(ILogger<ProductBuyerChangeValidatorService> logger, INotify buyerNotifyService)
		{
			this.logger = logger;
			this.buyerNotifyService = buyerNotifyService;
		}

		public void NotifyOnProductBuyerChange(Product initialState, Product finalState)
		{
			if (finalState.BuyerId != initialState.BuyerId)
			{
				buyerNotifyService.Notify(finalState.BuyerId, $"Product SKU '{finalState.SKU}' with Title '{finalState.Title}' has been assigned to you!");

				buyerNotifyService.Notify(initialState.BuyerId, $"Product SKU '{initialState.SKU}' with Title '{initialState.Title}' is no longer assigned to you!");
			}
		}
	}
}

