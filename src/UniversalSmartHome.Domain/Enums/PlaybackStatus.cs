namespace UniversalSmartHome.Domain.Enums;

/// <summary>
/// Represents the media playback status of media players and televisions.
/// </summary>
public enum PlaybackStatus
{
    Idle = 0,
    Playing,
    Paused,
    Stopped,
    Buffering
}
