namespace Naninovel.Metadata;

/// <summary>
/// Describes a Naninovel project.
/// </summary>
public class Project
{
    /// <summary>
    /// Actors available in the project.
    /// </summary>
    public Actor[] Actors { get; set; } = Array.Empty<Actor>();
    /// <summary>
    /// Script commands available in the project.
    /// </summary>
    public Command[] Commands { get; set; } = Array.Empty<Command>();
    /// <summary>
    /// Resources available in the project.
    /// </summary>
    public Resource[] Resources { get; set; } = Array.Empty<Resource>();
    /// <summary>
    /// Constants available in the project.
    /// </summary>
    public Constant[] Constants { get; set; } = Array.Empty<Constant>();
    /// <summary>
    /// Custom variables available in the project.
    /// </summary>
    public string[] Variables { get; set; } = Array.Empty<string>();
    /// <summary>
    /// Expression functions available in the project.
    /// </summary>
    public string[] Functions { get; set; } = Array.Empty<string>();
}
