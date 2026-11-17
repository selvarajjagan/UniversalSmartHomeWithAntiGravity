namespace UniversalSmartHome.Domain.ValueObjects;

/// <summary>
/// Represents an RGB color value for lighting capabilities.
/// </summary>
public readonly record struct RgbColor
{
    public byte Red { get; }
    public byte Green { get; }
    public byte Blue { get; }

    public RgbColor(byte red, byte green, byte blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }

    public static RgbColor White => new(255, 255, 255);
    public static RgbColor WarmWhite => new(255, 244, 229);
    public static RgbColor RedColor => new(255, 0, 0);
    public static RgbColor GreenColor => new(0, 255, 0);
    public static RgbColor BlueColor => new(0, 0, 255);

    public string ToHex() => $"#{Red:X2}{Green:X2}{Blue:X2}";

    public static RgbColor FromHex(string hex)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hex);
        var cleaned = hex.TrimStart('#');
        if (cleaned.Length != 6)
            throw new ArgumentException("Hex color must be 6 characters (e.g. #FFAA00).", nameof(hex));

        var r = Convert.ToByte(cleaned.Substring(0, 2), 16);
        var g = Convert.ToByte(cleaned.Substring(2, 2), 16);
        var b = Convert.ToByte(cleaned.Substring(4, 2), 16);
        return new RgbColor(r, g, b);
    }

    public override string ToString() => ToHex();
}
