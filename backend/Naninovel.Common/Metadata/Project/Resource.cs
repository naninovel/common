namespace Naninovel.Metadata;

/// <summary>
/// Describes a resource associated with the project.
/// </summary>
public class Resource
{
    /// <summary>
    /// Unique (type-wide) identifier of the resource.
    /// </summary>
    public string Path { get; set; } = string.Empty;
    /// <summary>
    /// Type of the resource: script, audio, spawned, etc.
    /// </summary>
    public string Type { get; set; } = string.Empty;
    /// <summary>
    /// Unique (project-wide) identifier of the asset associated
    /// with the resource or null when the resource is transient.
    /// </summary>
    public string? AssetId { get; set; }
}
