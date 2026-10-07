using WeatherApp.Models;

namespace WeatherApp.Interfaces;

internal interface IWeatherClient
{
    Task<WeatherData> GetWeatherAsync(City city, CancellationToken cancellationToken = default);
}
