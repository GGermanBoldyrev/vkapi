namespace WeatherApp.Exceptions;

internal sealed class WeatherResponseFormatException(Exception? innerException = null)
    : Exception("Unexpected weather response format.", innerException);
