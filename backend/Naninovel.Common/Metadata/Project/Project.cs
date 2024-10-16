using Naninovel.Parsing;

namespace Naninovel.Metadata;

/// <summary>
/// Describes a Naninovel project.
/// </summary>
public class Project
{
    /// <summary>
    /// Local resource path of the scenario script played when starting a new game.
    /// </summary>
    public string? EntryScript { get; set; }
    /// <summary>
    /// Local resource path of the scenario script played when entering title (main) menu.
    /// </summary>
    public string? TitleScript { get; set; }
    /// <summary>
    /// Actors available in the project.
    /// </summary>
    public Actor[] Actors { get; set; } = [];
    /// <summary>
    /// Script commands available in the project.
    /// </summary>
    public Command[] Commands { get; set; } = [];
    /// <summary>
    /// Resources available in the project.
    /// </summary>
    public Resource[] Resources { get; set; } = [];
    /// <summary>
    /// Constants available in the project.
    /// </summary>
    public Constant[] Constants { get; set; } = [];
    /// <summary>
    /// Custom variables available in the project.
    /// </summary>
    public string[] Variables { get; set; } = [];
    /// <summary>
    /// Expression functions available in the project.
    /// </summary>
    public Function[] Functions { get; set; } = [];
    /// <summary>
    /// Project-specific NaniScript syntax.
    /// </summary>
    public Syntax Syntax { get; set; } = Syntax.Default;
}
