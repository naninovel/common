namespace Naninovel.Metadata;

/// <summary>
/// Describes data container for a command parameter value.
/// </summary>
public enum ValueContainerType
{
    /// <summary>
    /// A single value.
    /// </summary>
    Single,
    /// <summary>
    /// List of single values.
    /// </summary>
    List,
    /// <summary>
    /// A key-value span with a string key; seperated by a dot.
    /// </summary>
    Named,
    /// <summary>
    /// List of named values.
    /// </summary>
    NamedList
}
