using UniversalSmartHome.Domain.ValueObjects;

namespace UniversalSmartHome.Domain.Devices;

/// <summary>
/// Represents a room, area, or zone within a smart home.
/// </summary>
public sealed class Room
{
    public RoomId Id { get; init; }
    public HomeId HomeId { get; init; }
    public string Name { get; private set; }
    public string? Icon { get; private set; }

    public Room(RoomId id, HomeId homeId, string name, string? icon = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        HomeId = homeId;
        Name = name;
        Icon = icon;
    }

    public void Update(string name, string? icon = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        Icon = icon;
    }
}
