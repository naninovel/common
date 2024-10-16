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
    /// Nesting properties of the command or null in case the command
    /// doesn't support nesting other commands under itself.
    /// </summary>
    public Nest? Nest { get; set; }
    /// <summary>
    /// Nature of script playback flow branching caused by the command execution
    /// or null when the command execution doesn't cause branching.
    /// </summary>
    public Branch? Branch { get; set; }
    /// <summary>
    /// Documentation for the command, when specified or null.
    /// </summary>
    public Documentation? Documentation { get; set; }
    /// <summary>
    /// List of parameters supported by the command.
    /// </summary>
    public Parameter[] Parameters { get; set; } = [];

    /// <summary>
    /// User-facing formatted identifier of the command.
    /// </summary>
    public string Label => BuildLabel(Alias, Id);
}
