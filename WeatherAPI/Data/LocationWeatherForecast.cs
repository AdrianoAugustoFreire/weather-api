using Microsoft.EntityFrameworkCore;

namespace WeatherAPI.Data
{
	[PrimaryKey(nameof(Latitude), nameof(Longitude), nameof(ForecastDate))]
	public class LocationWeatherForecast
	{
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public DateOnly ForecastDate { get; set; }
		public DateTime Created { get; set; }
		public int WeatherCode { get; set; }
		public float MaxTemp { get; set; }
		public float MinTemp { get; set; }
	}
}

