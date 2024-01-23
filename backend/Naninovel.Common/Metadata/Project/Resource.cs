namespace Naninovel.Metadata;

/// <summary>
/// Describes a resource associated with the project.
/// </summary>
public class Resource
{
    /// <summary>
    /// Unique (type-wide) identifier of the resource,
    /// scoped via path structure (separated with '/').
    /// </summary>
    public string Path { get; set; } = string.Empty;
    /// <summary>
    /// Type of the resource: script, audio, spawned prefab, etc.
    /// </summary>
    public string Type { get; set; } = string.Empty;
}
