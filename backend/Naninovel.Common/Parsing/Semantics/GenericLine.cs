using System.Text;

namespace Naninovel.Parsing;

/// <summary>
/// Represents script line primarily used to specify printed text,
/// but can also contain commands executed in the midst of printing.
/// </summary>
public class GenericLine (GenericPrefix? prefix,
    IReadOnlyList<IGenericContent> content, int indent = 0) : IScriptLine
{
    /// <summary>
    /// Optional (can be null) construct used to associated printed text with an author.
    /// </summary>
    public GenericPrefix? Prefix { get; } = prefix;
    /// <summary>
    /// The text to print; can contain inlined commands and expressions.
    /// </summary>
    public IReadOnlyList<IGenericContent> Content { get; } = content;
    public int Indent { get; } = indent;

    public GenericLine (IReadOnlyList<IGenericContent> content) : this(null, content) { }

    public override string ToString ()
    {
        var builder = new StringBuilder();
        if (Prefix != null) builder.Append(Prefix);
        foreach (var content in Content)
            builder.Append(content);
        return builder.ToString();
    }
}
