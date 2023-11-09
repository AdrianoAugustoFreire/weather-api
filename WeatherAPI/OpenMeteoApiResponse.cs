namespace WeatherAPI
{
	public class OpenMeteoApiResponse
	{
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public required DailyUnits DailyUnits { get; set; }
		public required DailyData Daily { get; set; }
	}

	public class DailyUnits
	{
		public required string Time { get; set; }
		public required string Weathercode { get; set; }
		public required string Temperature_2m_max { get; set; }
		public required string Temperature_2m_min { get; set; }
	}

	public class DailyData
	{
		public required List<string> Time { get; set; }
		public required List<int> Weathercode { get; set; }
		public required List<float> Temperature_2m_max { get; set; }
		public required List<float> Temperature_2m_min { get; set; }
	}
}

