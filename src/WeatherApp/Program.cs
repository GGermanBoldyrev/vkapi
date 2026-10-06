using WeatherApp.Cities;

namespace WeatherApp;

internal static class Program
{
    private static readonly string DefaultCitiesPath = Path.Combine(AppContext.BaseDirectory, "cities.txt");

    private static async Task<int> Main(string[] args)
    {
        string citiesPath = args.Length > 0 ? args[0] : DefaultCitiesPath;
        CityProvider cityProvider = new CityProvider(new FileCitySource(citiesPath));

        try
        {
            IReadOnlyList<City> cities = await cityProvider.GetCitiesAsync();

            foreach (City city in cities)
            {
                Console.WriteLine(city.Name);
            }

            return ExitCodes.Success;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Cannot read cities file '{citiesPath}': {ex.Message}");
            return ExitCodes.Failure;
        }
    }
}
