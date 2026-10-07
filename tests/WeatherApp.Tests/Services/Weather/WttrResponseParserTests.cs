using WeatherApp.Exceptions;
using WeatherApp.Models;
using WeatherApp.Services.Weather;

namespace WeatherApp.Tests.Services.Weather;

public sealed class WttrResponseParserTests
{
    private const string Response =
        """
        {
          "current_condition": [
            {
              "FeelsLikeC": "9",
              "lang_ru": [ { "value": "Облачно" } ],
              "temp_C": "12",
              "weatherDesc": [ { "value": "Partly cloudy" } ]
            }
          ],
          "nearest_area": [
            {
              "areaName": [ { "value": "Лахта" } ],
              "country": [ { "value": "Russia" } ],
              "region": [ { "value": "Saint Petersburg City" } ]
            }
          ],
          "request": [ { "query": "Lat 59.96 and Lon 30.16", "type": "LatLon" } ],
          "weather": [ { "avgtempC": "13", "hourly": [ { "tempC": "11" } ] } ]
        }
        """;

    private static readonly City City = new City("Saint-Petersburg");

    [Fact]
    public void Parse_ReturnsWeather_WithCityNameFromInput()
    {
        WeatherData weather = WttrResponseParser.Parse(City, Response);

        Assert.Equal(new WeatherData(City, new Country("Russia"), new Temperature(12)), weather);
    }

    [Fact]
    public void Parse_ParsesNegativeTemperature()
    {
        WeatherData weather = WttrResponseParser.Parse(City, Response.Replace("\"temp_C\": \"12\"", "\"temp_C\": \"-7\""));

        Assert.Equal(new Temperature(-7), weather.Temperature);
    }

    [Theory]
    [InlineData("""{ "weather": [] }""")]
    [InlineData("""{ "current_condition": [ { "temp_C": "warm" } ], "nearest_area": [ { "country": [ { "value": "Russia" } ] } ] }""")]
    [InlineData("location not found")]
    public void Parse_Throws_WhenResponseIsNotValidWeather(string response)
    {
        Assert.Throws<WeatherResponseFormatException>(() => WttrResponseParser.Parse(City, response));
    }
}
