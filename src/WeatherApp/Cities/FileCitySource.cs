namespace WeatherApp.Cities;

internal sealed class FileCitySource(string path) : ICitySource
{
    public async Task<IReadOnlyList<string>> GetCityNamesAsync(CancellationToken cancellationToken = default)
    {
        return await File.ReadAllLinesAsync(path, cancellationToken);
    }
}
