using ProductAPI.Model;

namespace ProductAPI.Services
{
	public interface IProductBuyerChangeValidatorService
	{
		void NotifyOnProductBuyerChange(Product initialState, Product finalState);
	}
}

