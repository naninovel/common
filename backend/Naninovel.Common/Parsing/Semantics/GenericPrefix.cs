using System.Text;

namespace Naninovel.Parsing;

/// <summary>
/// An optional construct of <see cref="GenericLine"/> used
/// to associate printed text with an author.
/// </summary>
public class GenericPrefix
{
    /// <summary>
    /// Author (actor) identifier to associate with the printed text.
    /// </summary>
    public string Author { get; }
    /// <summary>
    /// Optional (can be null) appearance of the author actor to apply.
    /// </summary>
    public string Appearance { get; }

    public GenericPrefix (string author, string appearance)
    {
        Author = author;
        Appearance = appearance;
    }

    public override string ToString ()
    {
        var builder = new StringBuilder(Author);
        if (!string.IsNullOrEmpty(Appearance))
        {
            builder.Append(Identifiers.AuthorAppearance);
            builder.Append(Appearance);
        }
        builder.Append(Identifiers.AuthorAssign);
        return builder.ToString();
    }
}
