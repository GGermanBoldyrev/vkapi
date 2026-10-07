namespace WeatherApp.Configuration;

internal sealed record LoaderOptions
{
    public int MaxParallelRequests { get; init; } = 10;
}
