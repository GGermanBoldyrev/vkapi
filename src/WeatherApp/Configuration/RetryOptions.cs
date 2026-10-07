namespace WeatherApp.Configuration;

internal sealed record RetryOptions
{
    public int MaxAttempts { get; init; } = 1;

    public TimeSpan Delay { get; init; } = TimeSpan.FromSeconds(1);
}
