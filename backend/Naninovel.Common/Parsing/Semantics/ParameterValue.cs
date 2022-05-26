using System.Collections.Generic;

namespace Naninovel.Parsing;

public class ParameterValue : LineText
{
    public List<LineText> Expressions { get; } = new();
    public bool Dynamic => Expressions.Count > 0;
}
