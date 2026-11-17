using UniversalSmartHome.Domain.ValueObjects;

namespace UniversalSmartHome.Domain.Devices;

/// <summary>
/// Represents a user home/location containing devices, rooms, and integrations.
/// </summary>
public sealed class Home
{
    private readonly List<Room> _rooms = [];

    public HomeId Id { get; init; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTimeOffset CreatedAt { get; init; }
    public IReadOnlyList<Room> Rooms => _rooms.AsReadOnly();

    public Home(HomeId id, string name, string? description = null, DateTimeOffset? createdAt = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        Name = name;
        Description = description;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }

    public void Rename(string name, string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        Description = description;
    }

    public void AddRoom(Room room)
    {
        ArgumentNullException.ThrowIfNull(room);
        if (_rooms.Any(r => r.Id == room.Id)) return;
        _rooms.Add(room);
    }

    public void RemoveRoom(RoomId roomId)
    {
        _rooms.RemoveAll(r => r.Id == roomId);
    }
}
