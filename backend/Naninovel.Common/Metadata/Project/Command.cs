namespace Naninovel.Metadata;

/// <summary>
/// Represents an action specified in scenario scripts.
/// </summary>
[Serializable]
public class Command
{
    /// <summary>
    /// Unique (project-wide) identifier of the command.
    /// </summary>
    public string Id = string.Empty;
    /// <summary>
    /// Optional short version of the identifier used to
    /// reference the command in scenario scripts.
    /// </summary>
    public string? Alias;
    /// <summary>
    /// Whether the command can be translated.
    /// </summary>
    public bool Localizable;
    /// <summary>
    /// Human-readable description of the command.
    /// </summary>
    public string? Summary;
    /// <summary>
    /// Additional, less important information about the command.
    /// </summary>
    public string? Remarks;
    /// <summary>
    /// Command usage examples in scenario script.
    /// </summary>
    public string? Examples;
    /// <summary>
    /// List of parameters the command supports.
    /// </summary>
    public Parameter[] Parameters = Array.Empty<Parameter>();

    /// <summary>
    /// User-facing formatted identifier of the command.
    /// </summary>
    public string Label => BuildLabel(Alias, Id);
}
