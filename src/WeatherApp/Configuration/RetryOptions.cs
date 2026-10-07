namespace WeatherApp.Configuration;

internal sealed record RetryOptions
{
    public int MaxAttempts { get; init; } = 3;

    public TimeSpan Delay { get; init; } = TimeSpan.FromSeconds(1);
}
