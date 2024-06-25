namespace Naninovel.Metadata;

/// <summary>
/// Pre-defined constants of any Naninovel project.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Default type of the character actors.
    /// </summary>
    public const string CharacterType = "Characters";
    /// <summary>
    /// Default type of the background actors.
    /// </summary>
    public const string BackgroundType = "Backgrounds";
    /// <summary>
    /// Default type of the scenario scripts.
    /// </summary>
    public const string ScriptsType = "Scripts";
    /// <summary>
    /// Flag representing any type.
    /// </summary>
    public const string WildcardType = "*";
    /// <summary>
    /// Subtype of the script part of an endpoint.
    /// </summary>
    public const string EndpointScript = "Script";
    /// <summary>
    /// Subtype of the label part of an endpoint.
    /// </summary>
    public const string EndpointLabel = "Label";
    /// <summary>
    /// Subtype of expression parameter context indicating that the expression is assignment.
    /// </summary>
    public const string Assignment = "Assignment";
    /// <summary>
    /// Subtype of expression parameter context indicating that the expression result
    /// is a condition for the associated command execution.
    /// </summary>
    public const string Condition = "Condition";
}
