namespace Naninovel.Metadata;

/// <summary>
/// Represents parameter of a script command.
/// </summary>
[Serializable]
public class Parameter
{
    /// <summary>
    /// Unique (command-wide) identifier of the parameter.
    /// </summary>
    public string Id = string.Empty;
    /// <summary>
    /// Optional short version of the identifier used to reference
    /// the parameter in scenario scripts.
    /// </summary>
    public string? Alias;
    /// <summary>
    /// Whether the parameter can be specified without the identifier.
    /// </summary>
    public bool Nameless;
    /// <summary>
    /// Whether the parameter is expected to always be specified.
    /// </summary>
    public bool Required;
    /// <summary>
    /// Whether the parameter can be translated.
    /// </summary>
    public bool Localizable;
    /// <summary>
    /// Type of the data container that stores value of the parameter.
    /// </summary>
    public ValueContainerType ValueContainerType;
    /// <summary>
    /// Type of the parameter value.
    /// </summary>
    public ValueType ValueType;
    /// <summary>
    /// Optional information about the context in which the value is used.
    /// </summary>
    /// <remarks>
    /// Multiple elements can be specified to map contexts to specific value indexes,
    /// in case the parameter value is of list or named container types.
    /// </remarks>
    public ValueContext?[]? ValueContext;
    /// <summary>
    /// The value of the parameter has when it's not explicitly specified in script.
    /// </summary>
    public string? DefaultValue;
    /// <summary>
    /// Human-readable description of the parameter.
    /// </summary>
    public string? Summary;

    /// <summary>
    /// User-facing formatted identifier of the parameter.
    /// </summary>
    public string Label => BuildLabel(Alias, Id);
    /// <summary>
    /// User-facing formatted name of the parameter's type.
    /// </summary>
    public string TypeLabel => BuildTypeLabel(ValueType, ValueContainerType);
}
