namespace ProductAPI.Model
{
	public class ProductDto
	{
		public string? SKU { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public string? BuyerId { get; set; }
		public bool Active { get; set; }
	}
}

