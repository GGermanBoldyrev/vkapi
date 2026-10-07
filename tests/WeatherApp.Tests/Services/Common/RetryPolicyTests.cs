using WeatherApp.Configuration;
using WeatherApp.Services.Common;

namespace WeatherApp.Tests.Services.Common;

public sealed class RetryPolicyTests
{
    private const int MaxAttempts = 3;

    private static readonly RetryPolicy Policy = new RetryPolicy(
        new RetryOptions { MaxAttempts = MaxAttempts, Delay = TimeSpan.Zero });

    [Fact]
    public async Task ExecuteAsync_RetriesAfterFailure_AndReturnsResult()
    {
        int calls = 0;

        int result = await Policy.ExecuteAsync(_ =>
        {
            calls++;

            return calls < MaxAttempts
                ? throw new InvalidOperationException("temporary failure")
                : Task.FromResult(42);
        });

        Assert.Equal(42, result);
        Assert.Equal(MaxAttempts, calls);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsLastError_WhenAllAttemptsFail()
    {
        int calls = 0;

        await Assert.ThrowsAsync<InvalidOperationException>(() => Policy.ExecuteAsync<int>(_ =>
        {
            calls++;

            throw new InvalidOperationException("permanent failure");
        }));

        Assert.Equal(MaxAttempts, calls);
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotRetry_WhenCancelled()
    {
        int calls = 0;

        await Assert.ThrowsAsync<OperationCanceledException>(() => Policy.ExecuteAsync<int>(_ =>
        {
            calls++;

            throw new OperationCanceledException();
        }));

        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task ExecuteAsync_ReportsEachRetry_WithExponentialDelay()
    {
        RetryPolicy policy = new RetryPolicy(new RetryOptions { MaxAttempts = 4, Delay = TimeSpan.FromMilliseconds(1) });
        List<(int Attempt, TimeSpan Delay)> retries = new List<(int Attempt, TimeSpan Delay)>();

        await Assert.ThrowsAsync<InvalidOperationException>(() => policy.ExecuteAsync<int>(
            _ => throw new InvalidOperationException("failure"),
            (attempt, _, delay) => retries.Add((attempt, delay))));

        Assert.Equal(
            [
                (1, TimeSpan.FromMilliseconds(1)),
                (2, TimeSpan.FromMilliseconds(2)),
                (3, TimeSpan.FromMilliseconds(4)),
            ],
            retries);
    }
}
