namespace Naninovel.Metadata;

/// <summary>
/// Describes a resource associated with the project.
/// </summary>
[Serializable]
public class Resource
{
    /// <summary>
    /// Unique (type-wide) identifier of the resource,
    /// scoped via path structure (seperated with '/').
    /// </summary>
    public string Path = string.Empty;
    /// <summary>
    /// Type of the resource: script, audio, spawned prefab, etc.
    /// </summary>
    public string Type = string.Empty;
}
