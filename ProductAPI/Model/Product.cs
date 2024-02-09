using Microsoft.EntityFrameworkCore;

namespace ProductAPI.Model
{
	[PrimaryKey(nameof(SKU))]
	public class Product
	{
		public required string SKU { get; set; }
		public required string Title { get; set; }
		public string? Description { get; set; }
		public required string BuyerId { get; set; }
		public bool Active { get; set; }
	}
}
