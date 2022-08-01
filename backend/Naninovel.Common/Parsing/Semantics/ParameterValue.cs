using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naninovel.Parsing;

public class ParameterValue : ILineComponent
{
    /// <summary>
    /// Value of the parameter; can contain expressions.
    /// </summary>
    public IReadOnlyList<IMixedValue> Mixed { get; }
    /// <summary>
    /// Whether value contains an expression and will be evaluated at runtime.
    /// </summary>
    public bool Dynamic => Mixed.Any(v => v is Expression);

    public ParameterValue (IReadOnlyList<IMixedValue> mixed)
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
