using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using WeatherAPI.Data;

namespace WheaterAPITests;

public class WeatherForecastDataServiceTests
{
	[Fact]
	public async Task DeleteLocation_LocationExists_DeletesData()
	{
		var dbContextMock = new Mock<WeatherForecastDbContext>();
		var loggerMock = new Mock<ILogger<WeatherForecastDataService>>();

		var latitudeToDelete = 33.656155;
		var longitudeToDelete = -117.89133;

		var locationWeatherData = new List<LocationWeatherForecast>
		{
			new LocationWeatherForecast { Latitude = latitudeToDelete, Longitude = longitudeToDelete },
        };

		dbContextMock
			.Setup(x => x.LocationWeatherData)
			.ReturnsDbSet(locationWeatherData);

		var service = new WeatherForecastDataService(dbContextMock.Object, loggerMock.Object);
		var deleteCount = await service.DeleteLocationAsync(latitudeToDelete, longitudeToDelete);

		Assert.Equal(locationWeatherData.Count, deleteCount);

		dbContextMock.Verify(db => db.RemoveRange(It.IsAny<IEnumerable<LocationWeatherForecast>>()), Times.Once);
		dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task GetLocations_ReturnsDistinctGeoLocations()
	{
		// Arrange
		var dbContextMock = new Mock<WeatherForecastDbContext>();
		var loggerMock = new Mock<ILogger<WeatherForecastDataService>>();

		var locationWeatherData = new List<LocationWeatherForecast>
		{
			new LocationWeatherForecast { Latitude = 33.656155, Longitude = -117.89133 },
			new LocationWeatherForecast { Latitude = 34.123, Longitude = -118.456 },
			new LocationWeatherForecast { Latitude = 34.123, Longitude = -118.456 },
			new LocationWeatherForecast { Latitude = -22.123, Longitude = -38.456 },
        };

		dbContextMock.Setup(db => db.LocationWeatherData)
			.ReturnsDbSet(locationWeatherData);
		
		var sut = new WeatherForecastDataService(dbContextMock.Object, loggerMock.Object);

		var result = await sut.GetDisticntLocationsAsync();

		Assert.NotNull(result);

		Assert.Collection(result,
			(item) =>
			{
				Assert.Equal(33.656155, item.Latitude);
				Assert.Equal(-117.89133, item.Longitude);
			},
			(item) =>
			{
				Assert.Equal(34.123, item.Latitude);
				Assert.Equal(-118.456, item.Longitude);
			},
			(item) =>
			{
				Assert.Equal(-22.123, item.Latitude);
				Assert.Equal(-38.456, item.Longitude);
			}
		);

		dbContextMock.Verify(db => db.LocationWeatherData, Times.Once);
	}

	[Fact]
	public async Task GetNewestForecastForLocationAsync_ReturnsNewestData()
	{
		var dbContextMock = new Mock<WeatherForecastDbContext>();
		var loggerMock = new Mock<ILogger<WeatherForecastDataService>>();

		var latitudeToQuery = 33.656155;
		var longitudeToQuery = -117.89133;

		var locationWeatherData = new List<LocationWeatherForecast>
		{
			new LocationWeatherForecast { Latitude = latitudeToQuery,
				Longitude = longitudeToQuery, Created = DateTime.Now },
        };

		dbContextMock.Setup(db => db.LocationWeatherData)
			.ReturnsDbSet(locationWeatherData);
		
		var service = new WeatherForecastDataService(dbContextMock.Object, loggerMock.Object);

		var result = await service.GetNewestForecastForLocationAsync(latitudeToQuery, longitudeToQuery);

		Assert.NotNull(result);
		Assert.Equal(locationWeatherData.OrderByDescending(b => b.Created).Take(7), result);

		dbContextMock.Verify(db => db.LocationWeatherData, Times.Once);
	}
}
