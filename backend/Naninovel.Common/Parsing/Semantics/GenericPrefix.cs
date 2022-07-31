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
    public PlainText Author { get; }
    /// <summary>
    /// Optional (can be null) appearance of the author actor to apply.
    /// </summary>
    public PlainText Appearance { get; }

    public GenericPrefix (PlainText author, PlainText appearance = null)
    {
        Author = author;
        Appearance = appearance;
    }

    public override string ToString ()
    {
        var builder = new StringBuilder(Author.Text);
        if (Appearance != null)
        {
            builder.Append(Identifiers.AuthorAppearance);
            builder.Append(Appearance);
        }
        builder.Append(Identifiers.AuthorAssign);
        return builder.ToString();
    }
}
