namespace Naninovel.Metadata;

/// <summary>
/// Describes type of the parameter value context.
/// </summary>
public enum ValueContextType
{
    /// <summary>
    /// A script expression containing <see cref="Project.Variables"/> and <see cref="Project.Functions"/>.
    /// </summary>
    Expression,
    /// <summary>
    /// A value from <see cref="Project.Constants"/>; name is specified via <see cref="ValueContext.SubType"/>.
    /// </summary>
    Constant,
    /// <summary>
    /// A value from <see cref="Project.Resources"/>; type is specified via <see cref="ValueContext.SubType"/>.
    /// </summary>
    Resource,
    /// <summary>
    /// Identifier of an actor from <see cref="Project.Actors"/>; type is specified via <see cref="ValueContext.SubType"/>.
    /// </summary>
    Actor,
    /// <summary>
    /// Actor appearance from <see cref="Project.Actors"/>; actor ID may be specified via <see cref="ValueContext.SubType"/>
    /// or otherwise is evaluated from another value with <see cref="Actor"/> context under the same parameter.
    /// </summary>
    Appearance,
    /// <summary>
    /// A color string in hex format.
    /// </summary>
    Color,
    /// <summary>
    /// Fixed-length array with named components; the components (separated by comma)
    /// are specified via <see cref="ValueContext.SubType"/>, eg "x,y,z".
    /// </summary>
    Vector
}
