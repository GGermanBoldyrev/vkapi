using WeatherApp.Configuration;
using WeatherApp.Exceptions;
using WeatherApp.Interfaces;
using WeatherApp.Models;
using WeatherApp.Services.Cities;
using WeatherApp.Services.Common;
using WeatherApp.Services.Reporting;
using WeatherApp.Services.Weather;

namespace WeatherApp.Tests;

public sealed class ApplicationTests
{
    [Fact]
    public async Task RunAsync_WritesCitiesThenCountries_AndReturnsSuccess()
    {
        using StringWriter output = new StringWriter();
        using StringWriter errors = new StringWriter();
        Application application = CreateApplication(output, errors, "Moscow", "Vienna", "Perm", "NhaTrang", "Villach");

        int exitCode = await application.RunAsync();

        Assert.Equal(ExitCodes.Success, exitCode);
        Assert.Equal(
            """
            Moscow, Russia +10 °C
            Vienna, Austria +13 °C
            Perm, Russia -5 °C
            NhaTrang, Vietnam +29 °C
            Villach, Austria +2 °C

            Russia — 2 cities, avg: +2.5 °C, min: -5 °C, max: +10 °C
            Austria — 2 cities, avg: +7.5 °C, min: +2 °C, max: +13 °C
            Vietnam — 1 city, avg: +29 °C, min: +29 °C, max: +29 °C

            """,
            output.ToString(),
            ignoreLineEndingDifferences: true);
        Assert.Empty(errors.ToString());
    }

    [Fact]
    public async Task RunAsync_WritesFailuresToErrors_AndReturnsFailure()
    {
        using StringWriter output = new StringWriter();
        using StringWriter errors = new StringWriter();
        Application application = CreateApplication(output, errors, "Moscow", "Atlantis");

        int exitCode = await application.RunAsync();

        Assert.Equal(ExitCodes.Failure, exitCode);
        Assert.Equal(
            """
            Moscow, Russia +10 °C

            Russia — 1 city, avg: +10 °C, min: +10 °C, max: +10 °C

            """,
            output.ToString(),
            ignoreLineEndingDifferences: true);
        Assert.Equal(
            """
            Atlantis: failed to get weather (unknown city)

            """,
            errors.ToString(),
            ignoreLineEndingDifferences: true);
    }

    private static Application CreateApplication(TextWriter output, TextWriter errors, params string[] cityNames)
    {
        CityProvider cityProvider = new CityProvider(new StubCitySource(cityNames));
        WeatherLoader weatherLoader = new WeatherLoader(
            new StubWeatherClient(),
            new RetryPolicy(new RetryOptions { MaxAttempts = 1 }),
            new LoaderOptions());

        return new Application(cityProvider, weatherLoader, new TextWeatherReportWriter(output, errors));
    }

    private sealed class StubCitySource(IReadOnlyList<string> names) : ICitySource
    {
        public Task<IReadOnlyList<string>> GetCityNamesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(names);
        }
    }

    private sealed class StubWeatherClient : IWeatherClient
    {
        private static readonly Dictionary<string, (string Country, double Celsius)> Known =
            new Dictionary<string, (string Country, double Celsius)>
            {
                ["Moscow"] = ("Russia", 10),
                ["Perm"] = ("Russia", -5),
                ["Vienna"] = ("Austria", 13),
                ["Villach"] = ("Austria", 2),
                ["NhaTrang"] = ("Vietnam", 29),
            };

        public Task<WeatherData> GetWeatherAsync(City city, CancellationToken cancellationToken = default)
        {
            if (!Known.TryGetValue(city.Name, out (string Country, double Celsius) weather))
            {
                throw new WeatherApiException("unknown city");
            }

            return Task.FromResult(new WeatherData(city, new Country(weather.Country), new Temperature(weather.Celsius)));
        }
    }
}
