using System.Collections.Generic;
using System.Text;

namespace Naninovel.Parsing;

public class GenericTextLine : LineContent, IScriptLine
{
    public GenericTextPrefix Prefix { get; } = new();
    public List<IGenericContent> Content { get; } = new();

    public override string ToString ()
    {
        var builder = new StringBuilder();
        builder.Append(Prefix);
        foreach (var content in Content)
            builder.Append(content);
        return builder.ToString();
    }
}
