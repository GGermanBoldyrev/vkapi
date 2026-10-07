using WeatherApp.Configuration;
using WeatherApp.Services.Cities;
using WeatherApp.Services.Common;
using WeatherApp.Services.Reporting;
using WeatherApp.Services.Weather;

namespace WeatherApp;

// Корень композиции: здесь создаются и связываются все сервисы приложения.
// Использовать только в точке входа; сами сервисы получают зависимости через конструкторы.
internal sealed class AppServices : IDisposable
{
    private readonly HttpClient _httpClient = new HttpClient();

    public AppServices(AppOptions options)
    {
        ProgressLog progressLog = new ProgressLog(Console.Error);

        CityProvider cityProvider = new CityProvider(new FileCitySource(options.CitiesFilePath));

        WttrWeatherClient weatherClient = new WttrWeatherClient(_httpClient, options.WeatherApi);
        RetryPolicy retryPolicy = new RetryPolicy(options.Retry);
        WeatherLoader weatherLoader = new WeatherLoader(weatherClient, retryPolicy, progressLog, options.Loader);

        TextWeatherReportWriter reportWriter = new TextWeatherReportWriter(Console.Out, Console.Error);

        Application = new Application(cityProvider, weatherLoader, reportWriter, progressLog);
    }

    public Application Application { get; }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
