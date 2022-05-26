namespace Naninovel.Metadata;

/// <summary>
/// Describes type of the parameter value context.
/// </summary>
public enum ValueContextType
{
    /// <summary>
    /// A script expression.
    /// </summary>
    Expression,
    /// <summary>
    /// Value from a project constant.
    /// </summary>
    Constant,
    /// <summary>
    /// A project resource.
    /// </summary>
    Resource,
    /// <summary>
    /// Identifier of an actor.
    /// </summary>
    Actor,
    /// <summary>
    /// Identifier of an actor appearance.
    /// </summary>
    Appearance
}
