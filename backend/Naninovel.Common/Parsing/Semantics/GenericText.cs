using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naninovel.Parsing;

/// <summary>
/// Part of <see cref="GenericLine"/> consisting of text to be printed at playback.
/// </summary>
public class GenericText : IGenericContent
{
    /// <summary>
    /// The text to print; can contain script expressions.
    /// </summary>
    public IReadOnlyList<IMixedValue> Text { get; }

    public GenericText (IEnumerable<IMixedValue> text)
    {
        Text = text.ToArray();
    }

    public override string ToString ()
    {
        var builder = new StringBuilder();
        foreach (var value in Text)
            builder.Append(value);
        return builder.ToString();
    }
}
