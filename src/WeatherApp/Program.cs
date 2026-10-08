using System.Text;

using WeatherApp.Configuration;

namespace WeatherApp;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        using CancellationTokenSource cancellation = new CancellationTokenSource();

        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cancellation.Cancel();
        };

        AppOptions options = AppOptionsFactory.Create(args);

        using AppServices services = new AppServices(options);

        try
        {
            return await services.Application.RunAsync(cancellation.Token);
        }
        catch (OperationCanceledException)
        {
            Console.Error.WriteLine("Cancelled.");
            return ExitCodes.Failure;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return ExitCodes.Failure;
        }
    }
}
