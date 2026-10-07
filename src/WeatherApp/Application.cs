using WeatherApp.Interfaces;
using WeatherApp.Models;
using WeatherApp.Services.Cities;
using WeatherApp.Services.Common;
using WeatherApp.Services.Reporting;
using WeatherApp.Services.Weather;

namespace WeatherApp;

internal sealed class Application(
    CityProvider cityProvider,
    WeatherLoader weatherLoader,
    IWeatherReportWriter reportWriter,
    ProgressLog progressLog)
{
    public async Task<int> RunAsync(CancellationToken cancellationToken = default)
    {
        progressLog.Step("Reading cities");
        IReadOnlyList<City> cities = await cityProvider.GetCitiesAsync(cancellationToken);

        progressLog.Step($"Loading weather, cities: {cities.Count}");
        IReadOnlyList<WeatherResult> results = await weatherLoader.LoadAsync(cities, cancellationToken);

        List<WeatherData> weather = results
            .Where(result => result.IsSuccess)
            .Select(result => result.Weather!)
            .ToList();

        IReadOnlyList<CountryStatistics> statistics = CountryStatisticsCalculator.Calculate(weather);

        progressLog.Step($"Loaded: {weather.Count} of {results.Count}");
        reportWriter.Write(results, statistics);

        return results.All(result => result.IsSuccess) ? ExitCodes.Success : ExitCodes.Failure;
    }
}
