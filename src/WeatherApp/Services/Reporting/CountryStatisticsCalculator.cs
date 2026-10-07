using WeatherApp.Models;

namespace WeatherApp.Services.Reporting;

internal static class CountryStatisticsCalculator
{
    // Страны идут в порядке первого появления в списке.
    public static IReadOnlyList<CountryStatistics> Calculate(IEnumerable<WeatherData> weather)
    {
        return weather
            .GroupBy(item => item.Country)
            .Select(group => new CountryStatistics(
                group.Key,
                group.Count(),
                new Temperature(group.Average(item => item.Temperature.Celsius)),
                new Temperature(group.Min(item => item.Temperature.Celsius)),
                new Temperature(group.Max(item => item.Temperature.Celsius))))
            .ToList();
    }
}
