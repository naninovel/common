using System.Collections.Generic;
using System.Text;

namespace Naninovel.Parsing;

public class Command : LineContent
{
    public LineText Identifier { get; } = new();
    public List<Parameter> Parameters { get; } = new();

    public override string ToString ()
    {
        var builder = new StringBuilder(Identifier);
        foreach (var parameter in Parameters)
            builder.Append(' ').Append(parameter);
        return builder.ToString();
    }
}
