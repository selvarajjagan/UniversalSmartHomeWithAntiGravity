using UniversalSmartHome.Domain.Enums;

namespace UniversalSmartHome.Domain.Capabilities;

/// <summary>
/// Capability for media playback controls (Play, Pause, Stop, Next, Previous).
/// </summary>
public sealed class MediaPlaybackCapability : DeviceCapability
{
    public const string CapabilityId = "media_playback";
    public const string CommandPlay = "Play";
    public const string CommandPause = "Pause";
    public const string CommandPlayPause = "PlayPause";
    public const string CommandStop = "Stop";
    public const string CommandNext = "Next";
    public const string CommandPrevious = "Previous";

    public const string PropertyPlaybackStatus = "PlaybackStatus";
    public const string PropertyMediaTitle = "MediaTitle";
    public const string PropertyMediaArtist = "MediaArtist";
    public const string PropertyMediaAlbum = "MediaAlbum";
    public const string PropertyMediaDurationSeconds = "MediaDurationSeconds";
    public const string PropertyMediaPositionSeconds = "MediaPositionSeconds";

    public bool SupportsNextPrevious { get; }
    public bool SupportsStop { get; }

    public MediaPlaybackCapability(bool supportsNextPrevious = true, bool supportsStop = true)
        : base(
            CapabilityId,
            "Media Playback",
            CapabilityType.Playback,
            GetCommands(supportsNextPrevious, supportsStop),
            [
                PropertyPlaybackStatus,
                PropertyMediaTitle,
                PropertyMediaArtist,
                PropertyMediaAlbum,
                PropertyMediaDurationSeconds,
                PropertyMediaPositionSeconds
            ])
    {
        SupportsNextPrevious = supportsNextPrevious;
        SupportsStop = supportsStop;
    }

    private static List<string> GetCommands(bool nextPrev, bool stop)
    {
        var list = new List<string> { CommandPlay, CommandPause, CommandPlayPause };
        if (stop) list.Add(CommandStop);
        if (nextPrev)
        {
            list.Add(CommandNext);
            list.Add(CommandPrevious);
        }
        return list;
    }
}
