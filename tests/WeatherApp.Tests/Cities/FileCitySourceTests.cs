using System.Text;
using WeatherApp.Cities;

namespace WeatherApp.Tests.Cities;

public sealed class FileCitySourceTests
{
    [Fact]
    public async Task GetCityNamesAsync_ReadsLines_FromFileWithBomAndWindowsLineEndings()
    {
        string path = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(path, "Moscow\r\nVienna\r\n", new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            FileCitySource source = new FileCitySource(path);

            IReadOnlyList<string> names = await source.GetCityNamesAsync();

            Assert.Equal(["Moscow", "Vienna"], names);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task GetCityNamesAsync_Throws_WhenFileDoesNotExist()
    {
        FileCitySource source = new FileCitySource(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));

        await Assert.ThrowsAsync<FileNotFoundException>(() => source.GetCityNamesAsync());
    }
}
