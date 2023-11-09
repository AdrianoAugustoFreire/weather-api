using Dawn;
using Newtonsoft.Json;
using WeatherAPI.Data;

namespace WeatherAPI
{
	public interface IWeatherForecastParserService
	{
		List<LocationWeatherForecast> Parse(string json);
	}

	public class OpenMeteoParserService : IWeatherForecastParserService
	{
		private readonly ILogger<OpenMeteoParserService> logger;

		public OpenMeteoParserService(ILogger<OpenMeteoParserService> logger)
		{
			this.logger = logger;
		}

		public List<LocationWeatherForecast> Parse(string json)
		{
			Guard.Argument(() => json).NotNull().NotEmpty();

			logger.LogDebug($"Received JSON payload for parsing: '{json}'");

			var apiResponse = JsonConvert.DeserializeObject<OpenMeteoApiResponse>(json)
				?? throw new ApplicationException($"Failed to serialize JSON response.");

			var locationWeatherList = new List<LocationWeatherForecast>();

			for (int i = 0; i < apiResponse.Daily.Time.Count; i++)
			{
				var locationWeather = new LocationWeatherForecast
				{
					Latitude = apiResponse.Latitude,
					Longitude = apiResponse.Longitude,
					ForecastDate = DateOnly.Parse(apiResponse.Daily.Time[i]),
					Created = DateTime.UtcNow,
					WeatherCode = apiResponse.Daily.Weathercode[i],
					MaxTemp = apiResponse.Daily.Temperature_2m_max[i],
					MinTemp = apiResponse.Daily.Temperature_2m_min[i]
				};
				locationWeatherList.Add(locationWeather);
			}

			return locationWeatherList;
		}
	}
}
