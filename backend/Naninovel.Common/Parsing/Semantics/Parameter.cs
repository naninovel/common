using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naninovel.Parsing;

/// <summary>
/// Parameter of a <see cref="Command"/> used to control the behaviour.
/// </summary>
public class Parameter
{
    /// <summary>
    /// Unique identifier of the parameter.
    /// Can be null in which case the parameter is considered nameless.
    /// </summary>
    /// <remarks>
    /// Not case-sensitive.
    /// In v1 can be either alias or name of the associated command field.
    /// </remarks>
    public string Identifier { get; }
    /// <summary>
    /// Value of the parameter; can contain expressions.
    /// </summary>
    public IReadOnlyList<IMixedValue> Value { get; }
    /// <summary>
    /// Whether the parameter doesn't have identifier specified.
    /// </summary>
    /// <remarks>
    /// Command can have a single nameless parameter.
    /// </remarks>
    public bool Nameless => string.IsNullOrEmpty(Identifier);
    /// <summary>
    /// Whether value of the parameter contains an expression and will be evaluated at runtime.
    /// </summary>
    public bool Dynamic => Value.Any(v => v is Expression);

    public Parameter (string identifier, IReadOnlyList<IMixedValue> value)
    {
        Identifier = identifier;
        Value = value;
    }

    public override string ToString ()
    {
        var builder = new StringBuilder();
        if (!Nameless)
        {
            builder.Append(Identifier);
            builder.Append(Identifiers.ParameterAssign);
        }
        foreach (var value in Value)
            builder.Append(value);
        return builder.ToString();
    }
}
