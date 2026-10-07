using WeatherApp.Interfaces;
using WeatherApp.Models;
using WeatherApp.Services.Cities;

namespace WeatherApp.Tests.Services.Cities;

public sealed class CityProviderTests
{
    [Fact]
    public async Task GetCitiesAsync_TrimsWhitespaceAroundNames()
    {
        IReadOnlyList<City> cities = await GetCitiesAsync("  Moscow ", "\tVienna\r");

        Assert.Equal(["Moscow", "Vienna"], cities.Select(city => city.Name));
    }

    [Fact]
    public async Task GetCitiesAsync_SkipsEmptyAndWhitespaceLines()
    {
        IReadOnlyList<City> cities = await GetCitiesAsync("Moscow", "", "   ", "Perm");

        Assert.Equal(["Moscow", "Perm"], cities.Select(city => city.Name));
    }

    [Fact]
    public async Task GetCitiesAsync_RemovesDuplicatesIgnoringCase_KeepsFirstSpelling()
    {
        IReadOnlyList<City> cities = await GetCitiesAsync("Moscow", "moscow", " MOSCOW ", "Perm");

        Assert.Equal(["Moscow", "Perm"], cities.Select(city => city.Name));
    }

    private static Task<IReadOnlyList<City>> GetCitiesAsync(params string[] names)
    {
        CityProvider provider = new CityProvider(new StubCitySource(names));

        return provider.GetCitiesAsync();
    }

    private sealed class StubCitySource(IReadOnlyList<string> names) : ICitySource
    {
        public Task<IReadOnlyList<string>> GetCityNamesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(names);
        }
    }
}
