namespace Naninovel.Metadata;

/// <summary>
/// Represents a constant list of values associated with a name.
/// </summary>
[Serializable]
public class Constant
{
    /// <summary>
    /// Identifier of the constant.
    /// </summary>
    public string Name = string.Empty;
    /// <summary>
    /// The list of available values.
    /// </summary>
    public string[] Values = Array.Empty<string>();
}
