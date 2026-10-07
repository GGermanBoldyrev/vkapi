namespace WeatherApp.Configuration;

internal sealed record AppOptions
{
    public string CitiesFilePath { get; init; } = Path.Combine(AppContext.BaseDirectory, "cities.txt");

    public WeatherApiOptions WeatherApi { get; init; } = new WeatherApiOptions();

    public RetryOptions Retry { get; init; } = new RetryOptions();

    public LoaderOptions Loader { get; init; } = new LoaderOptions();
}
