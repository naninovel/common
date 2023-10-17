namespace Naninovel.Metadata;

/// <summary>
/// Represents a constant list of values associated with a name.
/// </summary>
public class Constant
{
    /// <summary>
    /// Identifier of the constant.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The list of available values.
    /// </summary>
    public string[] Values { get; set; } = Array.Empty<string>();
}
