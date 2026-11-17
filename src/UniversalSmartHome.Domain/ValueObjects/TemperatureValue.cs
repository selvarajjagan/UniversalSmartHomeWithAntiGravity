namespace UniversalSmartHome.Domain.ValueObjects;

/// <summary>
/// Unit of temperature measurement.
/// </summary>
public enum TemperatureUnit
{
    Celsius,
    Fahrenheit
}

/// <summary>
/// Represents a temperature value with unit conversion support.
/// </summary>
public readonly record struct TemperatureValue
{
    public double Value { get; }
    public TemperatureUnit Unit { get; }

    public TemperatureValue(double value, TemperatureUnit unit = TemperatureUnit.Celsius)
    {
        Value = value;
        Unit = unit;
    }

    public static TemperatureValue FromCelsius(double celsius) => new(celsius, TemperatureUnit.Celsius);
    public static TemperatureValue FromFahrenheit(double fahrenheit) => new(fahrenheit, TemperatureUnit.Fahrenheit);

    public TemperatureValue ToCelsius() =>
        Unit == TemperatureUnit.Celsius
            ? this
            : new TemperatureValue(Math.Round((Value - 32) * 5 / 9, 2), TemperatureUnit.Celsius);

    public TemperatureValue ToFahrenheit() =>
        Unit == TemperatureUnit.Fahrenheit
            ? this
            : new TemperatureValue(Math.Round(Value * 9 / 5 + 32, 2), TemperatureUnit.Fahrenheit);

    public override string ToString() => $"{Value:0.#}°{(Unit == TemperatureUnit.Celsius ? "C" : "F")}";
}
