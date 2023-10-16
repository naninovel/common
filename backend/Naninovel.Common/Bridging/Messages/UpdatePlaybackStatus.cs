namespace Naninovel.Bridging;

/// <summary>
/// Sent by the server when script playback status is updated.
/// </summary>
[Serializable]
public class UpdatePlaybackStatus : IServerMessage
{
    /// <summary>
    /// The actual script playback status.
    /// </summary>
    public PlaybackStatus PlaybackStatus = null!;
}
