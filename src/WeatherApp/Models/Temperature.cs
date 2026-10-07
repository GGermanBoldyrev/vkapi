using System.Globalization;

namespace WeatherApp.Models;

internal sealed record Temperature(double Celsius)
{
    // Знак у положительных и отрицательных, ноль без знака; дробная часть — до одного знака: "+18 °C", "-7.5 °C", "0 °C".
    private const string Format = "+0.#;-0.#;0";

    public override string ToString()
    {
        // Округляем явно: по умолчанию .NET округляет половинки к чётному (6.25 -> 6.2).
        double rounded = Math.Round(Celsius, 1, MidpointRounding.AwayFromZero);

        return $"{rounded.ToString(Format, CultureInfo.InvariantCulture)} °C";
    }
}
