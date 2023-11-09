namespace WeatherAPI.Data
{
	public interface IWeatherForecastDataService
	{
		Task<List<GeoLocation>> GetDisticntLocationsAsync();
		Task<List<LocationWeatherForecast>> GetNewestForecastForLocationAsync(double latitude, double longitude);
		Task<int> AddWeatherDataAsync(List<LocationWeatherForecast> dailyLocationWeatherDates);
		Task<int> DeleteLocationAsync(double latitude, double longitude);
	}
}