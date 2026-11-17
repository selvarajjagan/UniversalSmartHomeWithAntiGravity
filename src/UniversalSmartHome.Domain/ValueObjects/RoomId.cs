namespace UniversalSmartHome.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a room or area.
/// </summary>
public readonly record struct RoomId
{
    public Guid Value { get; }

    public RoomId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("RoomId cannot be empty.", nameof(value));
        Value = value;
    }

    public static RoomId New() => new(Guid.NewGuid());
    public static RoomId From(Guid value) => new(value);
    public static RoomId Parse(string value) => new(Guid.Parse(value));

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(RoomId id) => id.Value;
    public static implicit operator RoomId(Guid value) => new(value);
}
