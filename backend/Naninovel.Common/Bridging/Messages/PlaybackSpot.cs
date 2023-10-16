namespace Naninovel.Bridging;

/// <summary>
/// Represents a navigation spot inside a scenario script.
/// </summary>
[Serializable]
public class PlaybackSpot
{
    /// <summary>
    /// Identifier of the scenario script.
    /// </summary>
    public string ScriptName = null!;
    /// <summary>
    /// Zero-based index of the script line.
    /// </summary>
    public int LineIndex;
    /// <summary>
    /// Zero-based index of an item inside the line.
    /// </summary>
    public int InlineIndex;
}
