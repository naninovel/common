namespace Naninovel.Metadata;

/// <summary>
/// Describes a Naninovel project.
/// </summary>
[Serializable]
public class Project
{
    /// <summary>
    /// Actors available in the project.
    /// </summary>
    public Actor[] Actors = Array.Empty<Actor>();
    /// <summary>
    /// Script commands available in the project.
    /// </summary>
    public Command[] Commands = Array.Empty<Command>();
    /// <summary>
    /// Resources available in the project.
    /// </summary>
    public Resource[] Resources = Array.Empty<Resource>();
    /// <summary>
    /// Constants available in the project.
    /// </summary>
    public Constant[] Constants = Array.Empty<Constant>();
    /// <summary>
    /// Custom variables available in the project.
    /// </summary>
    public string[] Variables = Array.Empty<string>();
    /// <summary>
    /// Expression functions available in the project.
    /// </summary>
    public string[] Functions = Array.Empty<string>();
}
