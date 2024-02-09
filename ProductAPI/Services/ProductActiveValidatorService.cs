using ProductAPI.Model;

namespace ProductAPI.Services
{
	public class ProductActiveValidatorService : IProductActiveValidatorService
	{
		private readonly ILogger<ProductActiveValidatorService> logger;
		private readonly INotify buyerNotifyService;

		public ProductActiveValidatorService(ILogger<ProductActiveValidatorService> logger, INotify buyerNotifyService)
		{
			this.logger = logger;
			this.buyerNotifyService = buyerNotifyService;
		}

		public void NotifyOnProductActiveChange(Product initialState, Product finalState)
		{
			if (initialState.Active == true && finalState.Active == false)
			{
				buyerNotifyService.Notify(finalState.BuyerId, $"Product SKU '{finalState.SKU}' with Title '{finalState.Title}' has been de-activated!");
			}
		}
	}
}

