using WeatherApp.Configuration;
using WeatherApp.Interfaces;
using WeatherApp.Models;
using WeatherApp.Services.Common;

namespace WeatherApp.Services.Weather;

internal sealed class WeatherLoader(IWeatherClient weatherClient, RetryPolicy retryPolicy, LoaderOptions options)
{
    // Результаты идут в том же порядке, что и города. Ошибка одного города не мешает остальным.
    public async Task<IReadOnlyList<WeatherResult>> LoadAsync(
        IReadOnlyList<City> cities,
        CancellationToken cancellationToken = default)
    {
        using SemaphoreSlim gate = new SemaphoreSlim(options.MaxParallelRequests);

        IEnumerable<Task<WeatherResult>> tasks = cities.Select(city => LoadOneAsync(city, gate, cancellationToken));

        return await Task.WhenAll(tasks);
    }

    private async Task<WeatherResult> LoadOneAsync(City city, SemaphoreSlim gate, CancellationToken cancellationToken)
    {
        await gate.WaitAsync(cancellationToken);

        try
        {
            WeatherData weather = await retryPolicy.ExecuteAsync(
                token => weatherClient.GetWeatherAsync(city, token),
                cancellationToken);

            return WeatherResult.Success(weather);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return WeatherResult.Failure(city, ex.Message);
        }
        finally
        {
            gate.Release();
        }
    }
}
