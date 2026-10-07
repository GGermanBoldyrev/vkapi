using System.Globalization;
using System.Text.Json;

using WeatherApp.Exceptions;
using WeatherApp.Models;

namespace WeatherApp.Services.Weather;

internal static class WttrResponseParser
{
    // Название города берём из исходного списка: wttr.in возвращает его локализованным или названием района.
    public static WeatherData Parse(City city, string json)
    {
        try
        {
            using JsonDocument document = JsonDocument.Parse(json);
            JsonElement root = document.RootElement;

            string? temperature = root.GetProperty("current_condition")[0].GetProperty("temp_C").GetString();
            string? country = root.GetProperty("nearest_area")[0].GetProperty("country")[0].GetProperty("value").GetString();

            if (!double.TryParse(temperature, NumberStyles.Float, CultureInfo.InvariantCulture, out double celsius)
                || !double.IsFinite(celsius)
                || string.IsNullOrWhiteSpace(country))
            {
                throw new WeatherResponseFormatException();
            }

            return new WeatherData(city, new Country(country), new Temperature(celsius));
        }
        catch (Exception ex) when (ex is JsonException or KeyNotFoundException or IndexOutOfRangeException or InvalidOperationException)
        {
            throw new WeatherResponseFormatException(ex);
        }
    }
}
