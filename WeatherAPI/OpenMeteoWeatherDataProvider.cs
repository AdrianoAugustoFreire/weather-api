using WeatherAPI.Data;

namespace WeatherAPI
{
	using System.Net.Http;
	using System.Threading.Tasks;
	using Dawn;

	public class OpenMeteoWeatherDataProvider : IWeatherDataProvider
	{
		private readonly ILogger logger;
		private readonly HttpClient httpClient;
		private readonly IWeatherForecastParserService weatherDataParserService;

		public OpenMeteoWeatherDataProvider(ILogger<OpenMeteoWeatherDataProvider> logger, HttpClient httpClient, IWeatherForecastParserService weatherDataParserService)
		{
			this.logger = logger;
			this.httpClient = httpClient;
			this.weatherDataParserService = weatherDataParserService;
		}

		public async Task<List<LocationWeatherForecast>> GetWeatherForecastAsync(double latitude, double longitude)
		{
			Guard.Argument(latitude, nameof(latitude)).InRange(-90, 90);
			Guard.Argument(longitude, nameof(longitude)).InRange(-180, 180);

			try
			{
				var uri = BuildUri(latitude, longitude);

				using HttpResponseMessage response = await httpClient.GetAsync(uri);

				response.EnsureSuccessStatusCode();

				var jsonResponse = await response.Content.ReadAsStringAsync();

				logger.LogDebug($"Retrieved response '{jsonResponse}' for request to uri '{uri}'");
				
				var apiResponse = weatherDataParserService.Parse(jsonResponse);

				return apiResponse;

			}
			catch (HttpRequestException ex)
			{
				throw new WeatherDataProviderException("Failed to retrive data from the Open Meteo API", ex);
			}
		}

		private string BuildUri(double latitude, double longitude)
		{
			return $"https://api.open-meteo.com/v1/forecast" +
				$"?longitude={longitude}" +
				$"&latitude={latitude}" +
				$"&daily=weathercode,temperature_2m_max,temperature_2m_min";

		}
	}
}
