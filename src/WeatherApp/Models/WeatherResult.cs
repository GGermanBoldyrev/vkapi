using System.Diagnostics.CodeAnalysis;

namespace WeatherApp.Models;

// Итог запроса по одному городу: либо погода, либо текст ошибки.
internal sealed record WeatherResult(City City, WeatherData? Weather, string? Error)
{
    [MemberNotNullWhen(true, nameof(Weather))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Weather is not null;

    public static WeatherResult Success(WeatherData weather)
    {
        return new WeatherResult(weather.City, weather, null);
    }

    public static WeatherResult Failure(City city, string error)
    {
        return new WeatherResult(city, null, error);
    }
}
