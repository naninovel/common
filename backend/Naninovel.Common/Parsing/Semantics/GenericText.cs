using System.Collections.Generic;

namespace Naninovel.Parsing;

public class GenericText : LineText, IGenericContent
{
    public List<LineText> Expressions { get; } = new();
}
