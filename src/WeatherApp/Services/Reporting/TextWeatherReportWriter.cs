using WeatherApp.Interfaces;
using WeatherApp.Models;

namespace WeatherApp.Services.Reporting;

internal sealed class TextWeatherReportWriter(TextWriter output, TextWriter errors) : IWeatherReportWriter
{
    public void Write(IReadOnlyList<WeatherResult> results, IReadOnlyList<CountryStatistics> statistics)
    {
        WriteCities(results);

        if (statistics.Count == 0)
        {
            return;
        }

        output.WriteLine();
        WriteStatistics(statistics);
    }

    private void WriteCities(IReadOnlyList<WeatherResult> results)
    {
        foreach (WeatherResult result in results)
        {
            if (result.IsSuccess)
            {
                WeatherData weather = result.Weather;
                output.WriteLine($"{weather.City.Name}, {weather.Country.Name} {weather.Temperature}");
            }
            else
            {
                errors.WriteLine($"{result.City.Name}: failed to get weather ({result.Error})");
            }
        }
    }

    private void WriteStatistics(IReadOnlyList<CountryStatistics> statistics)
    {
        foreach (CountryStatistics item in statistics)
        {
            string cities = item.CityCount == 1 ? "city" : "cities";

            output.WriteLine(
                $"{item.Country.Name} — {item.CityCount} {cities}, avg: {item.Average}, min: {item.Min}, max: {item.Max}");
        }
    }
}
