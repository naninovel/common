namespace Naninovel.Metadata;

/// <summary>
/// Represents context of parameter value.
/// </summary>
[Serializable]
public class ValueContext
{
    /// <summary>
    /// Type of the context.
    /// </summary>
    public ValueContextType Type;
    /// <summary>
    /// Optional, further specified type of the context.
    /// </summary>
    public string? SubType;
}
