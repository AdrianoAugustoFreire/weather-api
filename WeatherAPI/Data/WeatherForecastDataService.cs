using System;
using Dawn;
using Microsoft.EntityFrameworkCore;

namespace WeatherAPI.Data
{
	public class WeatherForecastDataService : IWeatherForecastDataService
	{
		private readonly ILogger<WeatherForecastDataService> logger;
		private readonly WeatherForecastDbContext context;

		public WeatherForecastDataService(WeatherForecastDbContext context, ILogger<WeatherForecastDataService> logger)
		{
			this.context = context;
			this.logger = logger;
		}

		public async Task<int> AddWeatherDataAsync(List<LocationWeatherForecast> locationWeatherForecasts)
		{
			Guard.Argument(locationWeatherForecasts, nameof(locationWeatherForecasts)).NotNull().NotEmpty();

			var recordsAdded = 0;
			foreach(var weatherForecast in locationWeatherForecasts)
			{
				if (context.LocationWeatherData.Contains(weatherForecast) == false) {
					context.LocationWeatherData.Add(weatherForecast);
					recordsAdded++;
				}
			}
			await context.SaveChangesAsync();
			logger.LogDebug($"Added {recordsAdded} new forecast records");

			return recordsAdded;
		}

		public async Task<int> DeleteLocationAsync(double latitude, double longitude)
		{
			var itemsToDelete = context.LocationWeatherData.Where(a => a.Latitude == latitude && a.Longitude == longitude);
			var deleteCount = 0;

			if (itemsToDelete != null)
			{
				deleteCount = itemsToDelete.Count();
				context.RemoveRange(itemsToDelete);
				await context.SaveChangesAsync();
				logger.LogDebug($"Removed {deleteCount} forecast records for latitude: '{latitude}' and longitude '{longitude}'");
			}

			return deleteCount;
		}

		public async Task<List<GeoLocation>> GetDisticntLocationsAsync()
		{
			var allData = await context.LocationWeatherData.ToListAsync();

			List<GeoLocation> distinctLocations = allData
				.GroupBy(p => new { p.Latitude, p.Longitude })
				.Select(g => g.First())
				.Select(a => new GeoLocation { Latitude = a.Latitude, Longitude = a.Longitude })
				.ToList();

			return distinctLocations;
		}

		public Task<List<LocationWeatherForecast>> GetNewestForecastForLocationAsync(double latitude, double longitude)
		{
			return context.LocationWeatherData
				.Where(a => a.Latitude == latitude && a.Longitude == longitude)
				.OrderByDescending(b => b.Created)
				.Take(7)
				.ToListAsync();
		}
	}
}

