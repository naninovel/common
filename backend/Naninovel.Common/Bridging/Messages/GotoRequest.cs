namespace Naninovel.Bridging;

/// <summary>
/// Asks server to navigate script playback to the specified spot.
/// </summary>
public class GotoRequest : IClientMessage
{
    /// <summary>
    /// The script playback spot to play.
    /// </summary>
    public PlaybackSpot PlaybackSpot { get; set; }
}
