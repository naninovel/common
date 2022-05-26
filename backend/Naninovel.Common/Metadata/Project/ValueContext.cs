namespace Naninovel.Metadata;

/// <summary>
/// Represents context of parameter value.
/// </summary>
public class ValueContext
{
    /// <summary>
    /// Type of the context.
    /// </summary>
    public ValueContextType Type { get; set; }
    /// <summary>
    /// Optional, further specified type of the context.
    /// </summary>
    public string SubType { get; set; }
}
