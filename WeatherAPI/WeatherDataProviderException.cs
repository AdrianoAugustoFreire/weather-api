using System;

namespace WeatherAPI;

public class WeatherDataProviderException : Exception
{
	public WeatherDataProviderException()
	{
	}

	public WeatherDataProviderException(string message)
		: base(message)
	{
	}

	public WeatherDataProviderException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}