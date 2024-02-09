namespace ProductAPI.Exceptions
{
	public class ProductSKUAlreadyExistsException : Exception
	{
		public ProductSKUAlreadyExistsException()
		{
		}

		public ProductSKUAlreadyExistsException(string message)
			: base(message)
		{
		}

		public ProductSKUAlreadyExistsException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
