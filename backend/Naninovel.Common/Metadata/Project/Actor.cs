namespace Naninovel.Metadata;

/// <summary>
/// Represents an entity manipulated via scenario scripts.
/// </summary>
[Serializable]
public class Actor
{
    /// <summary>
    /// Unique (project-wide) identifier of the actor.
    /// </summary>
    public string Id = string.Empty;
    /// <summary>
    /// Type of the actor: character, background, text printer, etc.
    /// </summary>
    public string Type = string.Empty;
    /// <summary>
    /// Collection of appearance identifiers supported by the actor.
    /// </summary>
    public string[] Appearances = Array.Empty<string>();
    /// <summary>
    /// Additional human-readable information about the actor.
    /// </summary>
    public string? Description;
}
