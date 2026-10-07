namespace WeatherApp.Models;

internal sealed record CountryStatistics(
    Country Country,
    int CityCount,
    Temperature Average,
    Temperature Min,
    Temperature Max);
