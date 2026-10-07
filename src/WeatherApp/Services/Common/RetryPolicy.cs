using WeatherApp.Configuration;

namespace WeatherApp.Services.Common;

internal sealed class RetryPolicy(RetryOptions options)
{
    // onRetry вызывается перед каждой паузой: номер неудачной попытки, ошибка и длительность паузы.
    public async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> action,
        Action<int, Exception, TimeSpan>? onRetry = null,
        CancellationToken cancellationToken = default)
    {
        for (int attempt = 1; ; attempt++)
        {
            try
            {
                return await action(cancellationToken);
            }
            catch (Exception ex) when (attempt < options.MaxAttempts && ex is not OperationCanceledException)
            {
                TimeSpan delay = options.Delay * Math.Pow(2, attempt - 1);

                onRetry?.Invoke(attempt, ex, delay);
                await Task.Delay(delay, cancellationToken);
            }
        }
    }
}
