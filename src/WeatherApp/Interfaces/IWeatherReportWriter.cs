using WeatherApp.Models;

namespace WeatherApp.Interfaces;

internal interface IWeatherReportWriter
{
    void Write(IReadOnlyList<WeatherResult> results, IReadOnlyList<CountryStatistics> statistics);
}
