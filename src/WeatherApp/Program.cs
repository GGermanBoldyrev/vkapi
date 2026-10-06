namespace WeatherApp;

internal static class Program
{
    private static async Task<int> Main()
    {
        await Console.Out.WriteLineAsync("WeatherApp started");

        return ExitCodes.Success;
    }
}
