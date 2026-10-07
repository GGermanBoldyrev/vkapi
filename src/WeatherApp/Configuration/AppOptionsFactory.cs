namespace WeatherApp.Configuration;

// Собирает настройки приложения из аргументов командной строки.
internal static class AppOptionsFactory
{
    public static AppOptions Create(string[] args)
    {
        AppOptions options = new AppOptions();

        if (args.Length > 0)
        {
            options = options with { CitiesFilePath = args[0] };
        }

        return options;
    }
}
