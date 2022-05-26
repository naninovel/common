using System.Text;

namespace Naninovel.Parsing;

public class GenericTextPrefix : LineContent
{
    public LineText AuthorIdentifier { get; } = new();
    public LineText AuthorAppearance { get; } = new();

    public override string ToString ()
    {
        if (Empty) return string.Empty;
        var builder = new StringBuilder();
        if (!AuthorIdentifier.Empty) builder.Append(AuthorIdentifier);
        if (!AuthorAppearance.Empty) builder.Append(Identifiers.AuthorAppearance).Append(AuthorAppearance);
        if (!AuthorIdentifier.Empty) builder.Append(Identifiers.AuthorAssign);
        return builder.ToString();
    }
}
