namespace WeatherApp.Cities;

// Источник отдаёт названия как есть: с дублями, пробелами и пустыми строками.
internal interface ICitySource
{
    Task<IReadOnlyList<string>> GetCityNamesAsync(CancellationToken cancellationToken = default);
}
