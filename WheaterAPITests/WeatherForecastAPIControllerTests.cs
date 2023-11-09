using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using WeatherAPI;
using WeatherAPI.Controllers;
using WeatherAPI.Data;

namespace WheaterAPITests;

public class WeatherForecastControllerTests
{
	[Fact]
	public async Task AddWeatherData_ValidData_AddsToDatabase()
	{
		var loggerMock = new Mock<ILogger<WeatherForecastDataService>>();

		var locationWeatherForecasts = new List<LocationWeatherForecast>
		{
			new LocationWeatherForecast {
				Created = DateTime.UtcNow,
				ForecastDate = DateOnly.FromDateTime(DateTime.UtcNow),
				Latitude = -22,
				Longitude = -33,
				MaxTemp = 30,
				MinTemp = 20,
				WeatherCode = 10 },
        };

		var dbContextMock = new Mock<WeatherForecastDbContext>();

		dbContextMock.Setup(x => x.LocationWeatherData).ReturnsDbSet(locationWeatherForecasts);

		var service = new WeatherForecastDataService(dbContextMock.Object, loggerMock.Object);

		var result = await service.AddWeatherDataAsync(locationWeatherForecasts);

		dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task Post_ValidParameters_ReturnsSuccess()
	{
		var loggerMock = new Mock<ILogger<WeatherForecastApiController>>();
		var weatherDataProviderMock = new Mock<IWeatherDataProvider>();
		var dailyLocationWeatherServiceMock = new Mock<IWeatherForecastDataService>();

		var controller = new WeatherForecastApiController(loggerMock.Object, weatherDataProviderMock.Object, dailyLocationWeatherServiceMock.Object);

		var latitude = 33.656155;
		var longitude = -117.89133;

		var sampleData = GetWeatherForecastSamples(latitude, longitude);

		weatherDataProviderMock.Setup(provider => provider.GetWeatherForecastAsync(latitude, longitude))
			.ReturnsAsync(sampleData);

		dailyLocationWeatherServiceMock
			.Setup(service => service.AddWeatherDataAsync(It.IsAny<List<LocationWeatherForecast>>()))
			.ReturnsAsync(1);
		
		var result = await controller.Post(latitude, longitude);

		Assert.IsType<OkObjectResult>(result);
	}

	[Fact]
	public async Task Get_ReturnsDistinctLocations()
	{
		var loggerMock = new Mock<ILogger<WeatherForecastApiController>>();
		var dailyLocationWeatherServiceMock = new Mock<IWeatherForecastDataService>();

		var controller = new WeatherForecastApiController(
			loggerMock.Object, null, dailyLocationWeatherServiceMock.Object);

		dailyLocationWeatherServiceMock.Setup(service => service.GetDisticntLocationsAsync())
			.ReturnsAsync(new List<GeoLocation> {
				new GeoLocation { Latitude = 33.656155, Longitude = -117.89133 },
				new GeoLocation { Latitude = 34.123, Longitude = -118.456 },
				new GeoLocation { Latitude = 34.123, Longitude = -118.456 },
				new GeoLocation { Latitude = -22.123, Longitude = -38.456 },
			 });

		var result = await controller.Get();

		Assert.IsType<OkObjectResult>(result);
	}

	[Fact]
	public async Task GetNewestWeatherForecastAsync_ValidParameters_ReturnsForecastData()
	{
		// Arrange
		var loggerMock = new Mock<ILogger<WeatherForecastApiController>>();
		var dailyLocationWeatherServiceMock = new Mock<IWeatherForecastDataService>();

		var controller = new WeatherForecastApiController(loggerMock.Object, null, dailyLocationWeatherServiceMock.Object);

		var latitude = 33.656155;
		var longitude = -117.89133;

		var weatherForecastReturnData = GetWeatherForecastSamples(latitude, longitude);

		dailyLocationWeatherServiceMock.Setup(service => service.GetNewestForecastForLocationAsync(latitude, longitude))
			.ReturnsAsync(weatherForecastReturnData);

		var result = await controller.GetNewestWeatherForecastAsync(latitude, longitude);

		Assert.IsType<OkObjectResult>(result);
	}

	[Fact]
	public async Task DeleteLocation_ValidParameters_ReturnsSuccess()
	{
		var loggerMock = new Mock<ILogger<WeatherForecastApiController>>();
		var dailyLocationWeatherServiceMock = new Mock<IWeatherForecastDataService>();

		var controller = new WeatherForecastApiController(loggerMock.Object, null, dailyLocationWeatherServiceMock.Object);

		var latitude = 33.656155;
		var longitude = -117.89133;

		var result = await controller.DeleteLocation(latitude, longitude);

		Assert.IsType<OkObjectResult>(result);
	}

	private List<LocationWeatherForecast> GetWeatherForecastSamples(double latitude, double longitude)
	{
		return new List<LocationWeatherForecast> {
			new LocationWeatherForecast {
				Created = DateTime.UtcNow.AddMinutes(10),
				ForecastDate = DateOnly.FromDateTime(DateTime.UtcNow),
				Latitude = latitude,
				Longitude = longitude,
				MaxTemp = 30,
				MinTemp = 20,
				WeatherCode = 10
			},
			new LocationWeatherForecast {
				Created = DateTime.UtcNow.AddMinutes(20),
				ForecastDate = DateOnly.FromDateTime(DateTime.UtcNow),
				Latitude = latitude,
				Longitude = longitude,
				MaxTemp = 30,
				MinTemp = 20,
				WeatherCode = 10
			},
			new LocationWeatherForecast {
				Created = DateTime.UtcNow.AddMinutes(15),
				ForecastDate = DateOnly.FromDateTime(DateTime.UtcNow),
				Latitude = latitude,
				Longitude = longitude,
				MaxTemp = 30,
				MinTemp = 20,
				WeatherCode = 10
			},
			new LocationWeatherForecast {
				Created = DateTime.UtcNow.AddMinutes(20),
				ForecastDate = DateOnly.FromDateTime(DateTime.UtcNow),
				Latitude = latitude,
				Longitude = longitude,
				MaxTemp = 30,
				MinTemp = 20,
				WeatherCode = 10
			}
		};
	}
}





