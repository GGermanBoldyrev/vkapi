using WeatherApp.Configuration;
using WeatherApp.Exceptions;
using WeatherApp.Interfaces;
using WeatherApp.Models;
using WeatherApp.Services.Common;
using WeatherApp.Services.Weather;

namespace WeatherApp.Tests.Services.Weather;

public sealed class WeatherLoaderTests
{
    private static readonly IReadOnlyList<City> Cities =
        [new City("Moscow"), new City("Vienna"), new City("Perm"), new City("Villach"), new City("Izhevsk")];

    private static readonly RetryPolicy NoRetries = new RetryPolicy(new RetryOptions { MaxAttempts = 1 });
    private static readonly ProgressLog NoProgress = new ProgressLog(TextWriter.Null);

    [Fact]
    public async Task LoadAsync_ReturnsResultsInCityOrder()
    {
        WeatherLoader loader = new WeatherLoader(new FakeWeatherClient(), NoRetries, NoProgress, new LoaderOptions { MaxParallelRequests = 3 });

        IReadOnlyList<WeatherResult> results = await loader.LoadAsync(Cities);

        Assert.Equal(Cities, results.Select(result => result.City));
        Assert.All(results, result => Assert.True(result.IsSuccess));
    }

    [Fact]
    public async Task LoadAsync_KeepsOtherCities_WhenOneFails()
    {
        WeatherLoader loader = new WeatherLoader(new FakeWeatherClient(failingCity: "Perm"), NoRetries, NoProgress, new LoaderOptions { MaxParallelRequests = 3 });

        IReadOnlyList<WeatherResult> results = await loader.LoadAsync(Cities);

        WeatherResult failed = Assert.Single(results, result => !result.IsSuccess);
        Assert.Equal("Perm", failed.City.Name);
        Assert.Equal("no weather for Perm", failed.Error);
    }

    [Fact]
    public async Task LoadAsync_DoesNotExceedParallelRequestLimit()
    {
        FakeWeatherClient client = new FakeWeatherClient();
        WeatherLoader loader = new WeatherLoader(client, NoRetries, NoProgress, new LoaderOptions { MaxParallelRequests = 2 });

        await loader.LoadAsync(Cities);

        Assert.Equal(2, client.MaxConcurrentCalls);
    }

    private sealed class FakeWeatherClient(string? failingCity = null) : IWeatherClient
    {
        private int _concurrentCalls;
        private int _maxConcurrentCalls;

        public int MaxConcurrentCalls => _maxConcurrentCalls;

        public async Task<WeatherData> GetWeatherAsync(City city, CancellationToken cancellationToken = default)
        {
            int current = Interlocked.Increment(ref _concurrentCalls);
            InterlockedMax(ref _maxConcurrentCalls, current);

            try
            {
                // Пауза нужна, чтобы запросы успели наложиться друг на друга.
                await Task.Delay(TimeSpan.FromMilliseconds(30), cancellationToken);

                if (city.Name == failingCity)
                {
                    throw new WeatherApiException($"no weather for {city.Name}");
                }

                return new WeatherData(city, new Country("Testland"), new Temperature(1));
            }
            finally
            {
                Interlocked.Decrement(ref _concurrentCalls);
            }
        }

        private static void InterlockedMax(ref int target, int value)
        {
            int snapshot;
            do
            {
                snapshot = Volatile.Read(ref target);
            }
            while (value > snapshot && Interlocked.CompareExchange(ref target, value, snapshot) != snapshot);
        }
    }
}
