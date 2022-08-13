using System;

namespace Naninovel.Metadata;

/// <summary>
/// Represents an action specified in scenario scripts.
/// </summary>
public class Command
{
    /// <summary>
    /// Unique (project-wide) identifier of the command.
    /// </summary>
    public string Id { get; set; } = string.Empty;
    /// <summary>
    /// Optional short version of the identifier used to
    /// reference the command in scenario scripts.
    /// </summary>
    public string? Alias { get; set; }
    /// <summary>
    /// Whether the command can be translated.
    /// </summary>
    public bool Localizable { get; set; }
    /// <summary>
    /// Human-readable description of the command.
    /// </summary>
    public string? Summary { get; set; }
    /// <summary>
    /// Additional, less important information about the command.
    /// </summary>
    public string? Remarks { get; set; }
    /// <summary>
    /// Command usage examples in scenario script.
    /// </summary>
    public string? Examples { get; set; }
    /// <summary>
    /// List of parameters the command supports.
    /// </summary>
    public Parameter[] Parameters { get; set; } = Array.Empty<Parameter>();

    /// <summary>
    /// User-facing formatted identifier of the command.
    /// </summary>
    public string Label => Utilities.BuildLabel(Alias, Id);
}
