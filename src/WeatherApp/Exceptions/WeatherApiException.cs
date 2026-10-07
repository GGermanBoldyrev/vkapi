namespace WeatherApp.Exceptions;

internal sealed class WeatherApiException(string message) : Exception(message);
