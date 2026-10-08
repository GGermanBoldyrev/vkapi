using WeatherApp.Configuration;
using WeatherApp.Exceptions;
using WeatherApp.Interfaces;
using WeatherApp.Models;

namespace WeatherApp.Services.Weather;

internal sealed class WttrWeatherClient(HttpClient httpClient, WeatherApiOptions options) : IWeatherClient
{
    private const string JsonFormat = "j1";

    // Особнность API - 500 и тело)
    private const string CityNotFoundBody = "location not found";

    public async Task<WeatherData> GetWeatherAsync(City city, CancellationToken cancellationToken = default)
    {
        Uri uri = new Uri(options.BaseUrl, $"{Uri.EscapeDataString(city.Name)}?format={JsonFormat}");

        using CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(options.RequestTimeout);

        try
        {
            using HttpResponseMessage response = await httpClient.GetAsync(uri, timeout.Token);
            string body = await response.Content.ReadAsStringAsync(timeout.Token);

            if (!response.IsSuccessStatusCode)
            {
                if (body.StartsWith(CityNotFoundBody, StringComparison.OrdinalIgnoreCase))
                {
                    throw new CityNotFoundException();
                }

                throw new WeatherApiException($"wttr.in returned {(int)response.StatusCode}: {body.Trim()}");
            }

            return WttrResponseParser.Parse(city, body);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new WeatherApiException($"wttr.in did not respond within {options.RequestTimeout.TotalSeconds:0.##} seconds");
        }
    }
}
