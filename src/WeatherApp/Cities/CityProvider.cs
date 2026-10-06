namespace WeatherApp.Cities;

internal sealed class CityProvider(ICitySource source)
{
    // Порядок сохраняется, из дублей остаётся первое написание.
    public async Task<IReadOnlyList<City>> GetCitiesAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<string> names = await source.GetCityNamesAsync(cancellationToken);

        return names
            .Select(name => name.Trim())
            .Where(name => name.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(name => new City(name))
            .ToList();
    }
}
