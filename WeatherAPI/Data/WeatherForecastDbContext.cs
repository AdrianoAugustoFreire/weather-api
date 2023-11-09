using Microsoft.EntityFrameworkCore;

namespace WeatherAPI.Data
{
	public class WeatherForecastDbContext: DbContext
	{
		public virtual DbSet<LocationWeatherForecast> LocationWeatherData { get; set; }

		public string DbPath { get; }

		public WeatherForecastDbContext() : base()
		{
			var folder = Environment.SpecialFolder.LocalApplicationData;
			var path = Environment.GetFolderPath(folder);
			DbPath = Path.Join(path, "WeatherAPI.sqlite");
		}

		public WeatherForecastDbContext(DbContextOptions<WeatherForecastDbContext> options): base(options)
		{
			var folder = Environment.SpecialFolder.LocalApplicationData;
			var path = Environment.GetFolderPath(folder);
			DbPath = Path.Join(path, "WeatherAPI.sqlite");
		}

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			options.UseSqlite($"Data Source={DbPath}");
		}
	}
}
