using Microsoft.Extensions.Logging;
using Moq;
using WeatherAPI;

namespace WheaterAPITests;

public class OpenMeteoParserServiceTests
{
	[Fact]
	public void Parse_ValidJson_ReturnsLocationWeatherForecasts()
	{
		var loggerMock = new Mock<ILogger<OpenMeteoParserService>>();
		var parserService = new OpenMeteoParserService(loggerMock.Object);

		var validJson = @"{
            ""latitude"": 33.656155,
            ""longitude"": -117.89133,
            ""generationtime_ms"": 0.1399517059326172,
            ""utc_offset_seconds"": 0,
            ""timezone"": ""GMT"",
            ""timezone_abbreviation"": ""GMT"",
            ""elevation"": 11.0,
            ""daily_units"": {
                ""time"": ""iso8601"",
                ""weathercode"": ""wmo code"",
                ""temperature_2m_max"": ""°C"",
                ""temperature_2m_min"": ""°C""
            },
            ""daily"": {
                ""time"": [
                    ""2023-11-06"",
                    ""2023-11-07""
                ],
                ""weathercode"": [1, 2],
                ""temperature_2m_max"": [20.0, 22.0],
                ""temperature_2m_min"": [10.0, 12.0]
            }
        }";

		var result = parserService.Parse(validJson);

		Assert.NotNull(result);
		Assert.Equal(2, result.Count);

		Assert.Equal(33.656155, result[0].Latitude);
		Assert.Equal(-117.89133, result[0].Longitude);
		Assert.Equal(20, result[0].MaxTemp);
		Assert.Equal(10, result[0].MinTemp);
		Assert.Equal(1, result[0].WeatherCode);

		Assert.Equal(33.656155, result[1].Latitude);
		Assert.Equal(-117.89133, result[1].Longitude);
		Assert.Equal(22, result[1].MaxTemp);
		Assert.Equal(12, result[1].MinTemp);
		Assert.Equal(2, result[1].WeatherCode);
	}

	[Fact]
	public void Parse_NullJson_ThrowsException()
	{
		var loggerMock = new Mock<ILogger<OpenMeteoParserService>>();
		var parserService = new OpenMeteoParserService(loggerMock.Object);

		Assert.Throws<ArgumentNullException>(() => parserService.Parse(null));
	}

	[Fact]
	public void Parse_EmptyJson_ThrowsException()
	{
		var loggerMock = new Mock<ILogger<OpenMeteoParserService>>();
		var parserService = new OpenMeteoParserService(loggerMock.Object);

		Assert.Throws<ArgumentException>(() => parserService.Parse(string.Empty));
	}
}
