namespace Naninovel.Metadata;

/// <summary>
/// A scenario script asset.
/// </summary>
public class Script
{
    /// <summary>
    /// Unique (project-wide) identifier of the script asset.
    /// </summary>
    public string Id { get; set; } = string.Empty;
    /// <summary>
    /// Local resource path of the script.
    /// </summary>
    public string Path { get; set; } = string.Empty;
}
