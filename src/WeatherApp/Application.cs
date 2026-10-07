using WeatherApp.Interfaces;
using WeatherApp.Models;
using WeatherApp.Services.Cities;
using WeatherApp.Services.Reporting;
using WeatherApp.Services.Weather;

namespace WeatherApp;

internal sealed class Application(CityProvider cityProvider, WeatherLoader weatherLoader, IWeatherReportWriter reportWriter)
{
    public async Task<int> RunAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<City> cities = await cityProvider.GetCitiesAsync(cancellationToken);
        IReadOnlyList<WeatherResult> results = await weatherLoader.LoadAsync(cities, cancellationToken);

        IReadOnlyList<WeatherData> weather = results
            .Where(result => result.IsSuccess)
            .Select(result => result.Weather!)
            .ToList();

        IReadOnlyList<CountryStatistics> statistics = CountryStatisticsCalculator.Calculate(weather);

        reportWriter.Write(results, statistics);

        return results.All(result => result.IsSuccess) ? ExitCodes.Success : ExitCodes.Failure;
    }
}
