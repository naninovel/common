using System.Text;

namespace Naninovel.Parsing;

/// <summary>
/// An optional construct of <see cref="GenericLine"/> used
/// to associate printed text with an author.
/// </summary>
public class GenericPrefix (PlainText author,
    PlainText? appearance = null) : ILineComponent
{
    /// <summary>
    /// Author (actor) identifier to associate with the printed text.
    /// </summary>
    public PlainText Author { get; } = author;
    /// <summary>
    /// Optional (can be null) appearance of the author actor to apply.
    /// </summary>
    public PlainText? Appearance { get; } = appearance;

    public override string ToString ()
    {
        var builder = new StringBuilder(Author.Text);
        if (Appearance != null)
        {
            builder.Append(Identifiers.Default.AuthorAppearance);
            builder.Append(Appearance);
        }
        builder.Append(Identifiers.Default.AuthorAssign);
        return builder.ToString();
    }
}
