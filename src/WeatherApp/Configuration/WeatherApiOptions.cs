namespace WeatherApp.Configuration;

internal sealed record WeatherApiOptions
{
    public Uri BaseUrl { get; init; } = new Uri("https://wttr.in");

    public TimeSpan RequestTimeout { get; init; } = TimeSpan.FromSeconds(10);
}
