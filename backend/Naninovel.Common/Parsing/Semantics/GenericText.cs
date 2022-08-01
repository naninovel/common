using System.Collections.Generic;
using System.Text;

namespace Naninovel.Parsing;

/// <summary>
/// Part of <see cref="GenericLine"/> consisting of text to be printed at playback.
/// </summary>
public class GenericText : ILineComponent, IGenericContent
{
    /// <summary>
    /// The text to print; can contain script expressions.
    /// </summary>
    public IReadOnlyList<IMixedValue> Mixed { get; }

    public GenericText (IReadOnlyList<IMixedValue> mixed)
    {
        Mixed = mixed;
    }

    public override string ToString ()
    {
        var builder = new StringBuilder();
        foreach (var value in Mixed)
            builder.Append(value);
        return builder.ToString();
    }
}
