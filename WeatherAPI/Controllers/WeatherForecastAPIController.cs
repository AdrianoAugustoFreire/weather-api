using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Data;

namespace WeatherAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class WeatherForecastApiController : ControllerBase
{
    private readonly ILogger<WeatherForecastApiController> logger;
	private readonly IWeatherDataProvider weatherDataProvider;
	private readonly IWeatherForecastDataService dailyLocationWeatherService;

    public WeatherForecastApiController(ILogger<WeatherForecastApiController> logger,
		IWeatherDataProvider weatherDataProvider,
		IWeatherForecastDataService dailyLocationWeatherService)
    {
        this.logger = logger;
        this.weatherDataProvider = weatherDataProvider;
		this.dailyLocationWeatherService = dailyLocationWeatherService;
    }

	[HttpPost(Name = "StoreWeatherForecast")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	public async Task<IActionResult> Post(double latitude, double longitude)
	{
		try
		{
			var added = await dailyLocationWeatherService
				.AddWeatherDataAsync(await weatherDataProvider.GetWeatherForecastAsync(latitude, longitude));

			return Ok($"{added} forecast records added.");
		}
		catch (Exception ex)
		{
			return StatusCode(500, ex.Message);
		}
	}

    [HttpGet("locations")]
    public async Task<ActionResult> Get()
    {
		try
		{
			var result = await dailyLocationWeatherService.GetDisticntLocationsAsync();
			if (result == null)
			{
				return NotFound();
			}
			else
			{
				return Ok(result);
			}
		}
		catch (Exception ex)
		{
			return StatusCode(500, ex.Message);
		}
	}

	[HttpGet(Name = "GetNewestWeatherForecast")]
	public async Task<IActionResult> GetNewestWeatherForecastAsync(double latitude, double longitude)
	{
		try
		{
			var result = await dailyLocationWeatherService
				.GetNewestForecastForLocationAsync(latitude, longitude);

			if (result == null)
			{
				return NotFound();
			}
			else
			{
				return Ok(result);
			}
		}
		catch (Exception ex)
		{
			return StatusCode(500, ex.Message);
		}
	}

	[HttpDelete(Name = "DeleteLocation")]
	public async Task<IActionResult> DeleteLocation(double latitude, double longitude)
	{
		try
		{
			await dailyLocationWeatherService.DeleteLocationAsync(latitude, longitude);
			return Ok(null);
		}
		catch (Exception ex)
		{
			return StatusCode(500, ex.Message);
		}
	}
}
