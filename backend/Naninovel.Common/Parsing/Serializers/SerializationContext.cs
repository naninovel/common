namespace Naninovel.Parsing;

/// <summary>
/// Context of the serialized script content.
/// </summary>
public struct SerializationContext
{
    /// <summary>
    /// Whether the content is part of a command parameter value
    /// and should be wrapped in quotes in case it contains whitespace.
    /// </summary>
    public bool ParameterValue { get; set; }
    /// <summary>
    /// Whether the content is the first element of generic line content
    /// and should has first author assign (':') escaped when it's not preceded by whitespace
    /// to ensure the content not later parsed as author identifier (generic line prefix).
    /// </summary>
    public bool FirstGenericContent { get; set; }
}
