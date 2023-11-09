using WeatherAPI.Data;

namespace WeatherAPI
{
	public interface IWeatherDataProvider
	{
		Task<List<LocationWeatherForecast>> GetWeatherForecastAsync(double latitude, double longitude);
	}
}

