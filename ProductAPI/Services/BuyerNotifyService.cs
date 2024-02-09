using System.Diagnostics;

namespace ProductAPI.Services
{
	public class BuyerNotifyService : INotify
	{
		private readonly ILogger<BuyerNotifyService> logger;

		public BuyerNotifyService(ILogger<BuyerNotifyService> logger)
		{
			this.logger = logger;
		}

		public void Notify(string userId, string message)
		{
			Debug.WriteLine($"Sending message...{userId} - {message}");
		}
	}
}
