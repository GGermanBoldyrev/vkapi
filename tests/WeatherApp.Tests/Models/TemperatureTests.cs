using WeatherApp.Models;

namespace WeatherApp.Tests.Models;

public sealed class TemperatureTests
{
    [Theory]
    [InlineData(18, "+18 °C")]
    [InlineData(-7, "-7 °C")]
    [InlineData(0, "0 °C")]
    [InlineData(7.5, "+7.5 °C")]
    [InlineData(-7.5, "-7.5 °C")]
    [InlineData(6.25, "+6.3 °C")]
    [InlineData(-0.04, "0 °C")]
    public void ToString_FormatsWithSignAndUpToOneDecimal(double celsius, string expected)
    {
        Assert.Equal(expected, new Temperature(celsius).ToString());
    }
}
