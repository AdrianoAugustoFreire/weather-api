using ProductAPI.Model;

namespace ProductAPI.Services
{
	public interface IProductActiveValidatorService
	{
		void NotifyOnProductActiveChange(Product initialState, Product finalState);
	}
}

