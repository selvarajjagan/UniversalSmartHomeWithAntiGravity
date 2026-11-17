namespace UniversalSmartHome.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a device connector.
/// </summary>
public readonly record struct ConnectorId
{
    public Guid Value { get; }

    public ConnectorId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ConnectorId cannot be empty.", nameof(value));
        Value = value;
    }

    public static ConnectorId New() => new(Guid.NewGuid());
    public static ConnectorId From(Guid value) => new(value);
    public static ConnectorId Parse(string value) => new(Guid.Parse(value));

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(ConnectorId id) => id.Value;
    public static implicit operator ConnectorId(Guid value) => new(value);
}
