namespace UniversalSmartHome.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a home.
/// </summary>
public readonly record struct HomeId
{
    public Guid Value { get; }

    public HomeId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("HomeId cannot be empty.", nameof(value));
        Value = value;
    }

    public static HomeId New() => new(Guid.NewGuid());
    public static HomeId From(Guid value) => new(value);
    public static HomeId Parse(string value) => new(Guid.Parse(value));

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(HomeId id) => id.Value;
    public static implicit operator HomeId(Guid value) => new(value);
}
