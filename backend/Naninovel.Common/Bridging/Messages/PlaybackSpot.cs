namespace Naninovel.Bridging;

/// <summary>
/// Represents a navigation spot inside a scenario script.
/// </summary>
public class PlaybackSpot
{
    /// <summary>
    /// Unique (project-wide) local resource path of the scenario script.
    /// </summary>
    public string ScriptPath { get; set; } = null!;
    /// <summary>
    /// Zero-based index of the script line.
    /// </summary>
    public int LineIndex { get; set; }
    /// <summary>
    /// Zero-based index of an item inside the line.
    /// </summary>
    public int InlineIndex { get; set; }
}
