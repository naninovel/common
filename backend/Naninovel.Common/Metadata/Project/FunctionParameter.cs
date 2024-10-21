namespace Naninovel.Metadata;

/// <summary>
/// Parameter of a <see cref="Function"/>.
/// </summary>
public class FunctionParameter
{
    /// <summary>
    /// Identifier of the parameter.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Type of the parameter value.
    /// </summary>
    public ValueType Type { get; set; }
    /// <summary>
    /// Optional information about the context in which the value is used.
    /// </summary>
    public ValueContext? Context { get; set; }
    /// <summary>
    /// Whether the parameter accepts multiple values of the same type.
    /// Expected to always be the last parameter in function.
    /// </summary>
    public bool Variadic { get; set; }
}
