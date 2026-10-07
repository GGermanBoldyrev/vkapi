using System.Net;

using WeatherApp.Configuration;
using WeatherApp.Exceptions;
using WeatherApp.Models;
using WeatherApp.Services.Weather;

namespace WeatherApp.Tests.Services.Weather;

public sealed class WttrWeatherClientTests
{
    private const string Body =
        """
        {
          "current_condition": [ { "temp_C": "-3" } ],
          "nearest_area": [ { "areaName": [ { "value": "Лахта" } ], "country": [ { "value": "Russia" } ] } ]
        }
        """;

    private static readonly City City = new City("Saint Petersburg");

    [Fact]
    public async Task GetWeatherAsync_RequestsEncodedCityInJsonFormat_AndKeepsCityNameFromInput()
    {
        StubHandler handler = new StubHandler(_ => Task.FromResult(Respond(HttpStatusCode.OK, Body)));
        using HttpClient http = new HttpClient(handler);
        WttrWeatherClient client = new WttrWeatherClient(http, new WeatherApiOptions());

        WeatherData weather = await client.GetWeatherAsync(City);

        Assert.Equal("https://wttr.in/Saint%20Petersburg?format=j1", handler.RequestedUri?.AbsoluteUri);
        Assert.Equal(new WeatherData(City, new Country("Russia"), new Temperature(-3)), weather);
    }

    [Fact]
    public async Task GetWeatherAsync_Throws_WhenStatusIsNotSuccess()
    {
        StubHandler handler = new StubHandler(_ => Task.FromResult(Respond(HttpStatusCode.InternalServerError, "location not found")));
        using HttpClient http = new HttpClient(handler);
        WttrWeatherClient client = new WttrWeatherClient(http, new WeatherApiOptions());

        WeatherApiException exception = await Assert.ThrowsAsync<WeatherApiException>(() => client.GetWeatherAsync(City));

        Assert.Equal("wttr.in returned 500: location not found", exception.Message);
    }

    [Fact]
    public async Task GetWeatherAsync_Throws_WhenServerDoesNotRespondInTime()
    {
        StubHandler handler = new StubHandler(async cancellationToken =>
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            return Respond(HttpStatusCode.OK, Body);
        });
        using HttpClient http = new HttpClient(handler);
        WeatherApiOptions options = new WeatherApiOptions { RequestTimeout = TimeSpan.FromMilliseconds(50) };
        WttrWeatherClient client = new WttrWeatherClient(http, options);

        WeatherApiException exception = await Assert.ThrowsAsync<WeatherApiException>(() => client.GetWeatherAsync(City));

        Assert.Contains("did not respond", exception.Message);
    }

    private static HttpResponseMessage Respond(HttpStatusCode status, string body)
    {
        return new HttpResponseMessage(status) { Content = new StringContent(body) };
    }

    // Подменяет сеть: HttpClient отдаёт то, что вернёт переданная функция.
    private sealed class StubHandler(Func<CancellationToken, Task<HttpResponseMessage>> respond) : HttpMessageHandler
    {
        public Uri? RequestedUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestedUri = request.RequestUri;

            return respond(cancellationToken);
        }
    }
}
