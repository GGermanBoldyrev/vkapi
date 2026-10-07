using System.Text;

using WeatherApp.Configuration;

namespace WeatherApp;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        AppOptions options = AppOptionsFactory.Create(args);

        using AppServices services = new AppServices(options);

        try
        {
            return await services.Application.RunAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return ExitCodes.Failure;
        }
    }
}
