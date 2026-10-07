using WeatherApp.Models;
using WeatherApp.Services.Reporting;

namespace WeatherApp.Tests.Services.Reporting;

public sealed class CountryStatisticsCalculatorTests
{
    private static readonly Country Russia = new Country("Russia");
    private static readonly Country Austria = new Country("Austria");

    [Fact]
    public void Calculate_GroupsByCountry_InOrderOfFirstAppearance()
    {
        IReadOnlyList<WeatherData> weather =
        [
            Weather("Moscow", Russia, 10),
            Weather("Vienna", Austria, 13),
            Weather("Perm", Russia, 5),
            Weather("Villach", Austria, 2),
            Weather("Izhevsk", Russia, 6),
        ];

        IReadOnlyList<CountryStatistics> statistics = CountryStatisticsCalculator.Calculate(weather);

        Assert.Equal(
            [
                new CountryStatistics(Russia, 3, new Temperature(7), new Temperature(5), new Temperature(10)),
                new CountryStatistics(Austria, 2, new Temperature(7.5), new Temperature(2), new Temperature(13)),
            ],
            statistics);
    }

    private static WeatherData Weather(string city, Country country, double celsius)
    {
        return new WeatherData(new City(city), country, new Temperature(celsius));
    }
}
