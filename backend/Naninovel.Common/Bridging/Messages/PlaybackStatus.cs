namespace Naninovel.Bridging;

/// <summary>
/// Represents a script playback status.
/// </summary>
[Serializable]
public class PlaybackStatus
{
    /// <summary>
    /// Whether a script is being played.
    /// </summary>
    public bool Playing;
    /// <summary>
    /// Current script playback spot, if any.
    /// </summary>
    public PlaybackSpot PlayedSpot = null!;
}
