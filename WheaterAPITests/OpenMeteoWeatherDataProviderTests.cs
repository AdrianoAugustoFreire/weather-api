using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using WeatherAPI;
using WeatherAPI.Data;

namespace WheaterAPITests;

public class OpenMeteoWeatherDataProviderTests
{
	[Fact]
	public async Task GetWeatherForecastAsync_Success_ReturnsData()
	{
		var weatherParserMock = new Mock<IWeatherForecastParserService>();

		var latitude = 33.656155;
		var longitude = -117.89133;
		var responseData = "{\"latitude\":33.656155,\"longitude\":-117.89133,\"generationtime_ms\":0.9659528732299805,\"utc_offset_seconds\":0,\"timezone\":\"GMT\",\"timezone_abbreviation\":\"GMT\",\"elevation\":23.0,\"daily_units\":{\"time\":\"iso8601\",\"weathercode\":\"wmo code\",\"temperature_2m_max\":\"°C\",\"temperature_2m_min\":\"°C\"},\"daily\":{\"time\":[\"2023-11-09\",\"2023-11-10\",\"2023-11-11\",\"2023-11-12\",\"2023-11-13\",\"2023-11-14\",\"2023-11-15\"],\"weathercode\":[0,3,0,1,3,3,3],\"temperature_2m_max\":[27.3,25.5,26.1,26.3,25.3,25.2,23.3],\"temperature_2m_min\":[14.3,9.6,9.4,18.5,18.0,18.5,18.2]}}";
		var expectedApiResponse = new List<LocationWeatherForecast> {
			new LocationWeatherForecast {
				Created = DateTime.UtcNow,
				ForecastDate = DateOnly.FromDateTime(DateTime.Now),
				Latitude = 33.656155,
				Longitude = -117.69133,
				MaxTemp = 20,
				MinTemp = 10,
				WeatherCode = 22 },
			new LocationWeatherForecast {
				Created = DateTime.UtcNow,
				ForecastDate = DateOnly.FromDateTime(DateTime.Now + TimeSpan.FromDays(1)),
				Latitude = 33.656155,
				Longitude = -117.69133,
				MaxTemp = 30,
				MinTemp = 11,
				WeatherCode = 22 },
			new LocationWeatherForecast {
				Created = DateTime.UtcNow,
				ForecastDate = DateOnly.FromDateTime(DateTime.Now + TimeSpan.FromDays(2)),
				Latitude = 33.656155,
				Longitude = -117.69133,
				MaxTemp = 22,
				MinTemp = 12,
				WeatherCode = 22 },
			new LocationWeatherForecast {
				Created = DateTime.UtcNow,
				ForecastDate = DateOnly.FromDateTime(DateTime.Now + TimeSpan.FromDays(3)),
				Latitude = 33.656155,
				Longitude = -117.69133,
				MaxTemp = 23,
				MinTemp = 13,
				WeatherCode = 22 },
			new LocationWeatherForecast {
				Created = DateTime.UtcNow,
				ForecastDate = DateOnly.FromDateTime(DateTime.Now + TimeSpan.FromDays(4)),
				Latitude = 33.656155,
				Longitude = -117.69133,
				MaxTemp = 24,
				MinTemp = 15,
				WeatherCode = 25 },
			new LocationWeatherForecast {
				Created = DateTime.UtcNow,
				ForecastDate = DateOnly.FromDateTime(DateTime.Now + TimeSpan.FromDays(5)),
				Latitude = 33.656155,
				Longitude = -117.69133,
				MaxTemp = 25,
				MinTemp = 15,
				WeatherCode = 25 },
			new LocationWeatherForecast {
				Created = DateTime.UtcNow,
				ForecastDate = DateOnly.FromDateTime(DateTime.Now + TimeSpan.FromDays(6)),
				Latitude = 33.656155,
				Longitude = -117.69133,
				MaxTemp = 26,
				MinTemp = 16,
				WeatherCode = 26 },
		};

		var httpClientMock = BuildMockableHttpClient(responseData);

		weatherParserMock
			.Setup(parser => parser.Parse(responseData))
			.Returns(expectedApiResponse);

		var sut = new OpenMeteoWeatherDataProvider(
			Mock.Of<ILogger<OpenMeteoWeatherDataProvider>>(),
			httpClientMock,
			weatherParserMock.Object
		);

		var result = await sut.GetWeatherForecastAsync(latitude, longitude);

		Assert.NotNull(result);
		Assert.Equal(expectedApiResponse, result);

		weatherParserMock.Verify(parser => parser.Parse(responseData), Times.Once);
	}

	[Fact]
	public async Task GetWeatherForecastAsync_InvalidLatitude_ThrowsException()
	{
		var responseData = "No data";
		var weatherParserMock = new Mock<IWeatherForecastParserService>();
		var httpClientMock = BuildMockableHttpClient(responseData);

		weatherParserMock
			.Setup(parser => parser.Parse(responseData))
			.Returns(new List<LocationWeatherForecast>());

		var sut = new OpenMeteoWeatherDataProvider(
			Mock.Of<ILogger<OpenMeteoWeatherDataProvider>>(),
			httpClientMock,
			weatherParserMock.Object
		);

		await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.GetWeatherForecastAsync(-99, 20));
	}

	[Fact]
	public async Task GetWeatherForecastAsync_InvalidLongitude_ThrowsException()
	{
		var responseData = "No data";
		var weatherParserMock = new Mock<IWeatherForecastParserService>();
		var httpClientMock = BuildMockableHttpClient(responseData);

		weatherParserMock
			.Setup(parser => parser.Parse(responseData))
			.Returns(new List<LocationWeatherForecast>());

		var sut = new OpenMeteoWeatherDataProvider(
			Mock.Of<ILogger<OpenMeteoWeatherDataProvider>>(),
			httpClientMock,
			weatherParserMock.Object
		);

		await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.GetWeatherForecastAsync(-117, 290));
	}

	private HttpClient BuildMockableHttpClient(string responseData)
	{
		var mockHttp = new MockHttpMessageHandler();

		mockHttp.When("https://api.open-meteo.com/v1/forecast*")
				.Respond("application/json", responseData);

		var httpClientMock = new HttpClient(mockHttp);

		return httpClientMock;
	} 
}