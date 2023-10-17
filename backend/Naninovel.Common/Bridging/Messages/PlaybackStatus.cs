namespace Naninovel.Bridging;

/// <summary>
/// Represents a script playback status.
/// </summary>
public class PlaybackStatus
{
    /// <summary>
    /// Whether a script is being played.
    /// </summary>
    public bool Playing { get; set; }
    /// <summary>
    /// Current script playback spot, if any.
    /// </summary>
    public PlaybackSpot PlayedSpot { get; set; } = null!;
}
