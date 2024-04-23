using System.Text;

namespace Naninovel.Parsing;

/// <summary>
/// Parameter of a <see cref="Command"/> used to control the behaviour.
/// </summary>
public class Parameter (PlainText? identifier, MixedValue value)
    : ILineComponent
{
    /// <summary>
    /// Unique identifier of the parameter.
    /// Can be null in which case the parameter is considered nameless.
    /// </summary>
    /// <remarks>
    /// Not case-sensitive.
    /// In v1 can be either alias or name of the associated command field.
    /// </remarks>
    public PlainText? Identifier { get; } = identifier;
    /// <summary>
    /// Value of the parameter; can contain expressions.
    /// </summary>
    public MixedValue Value { get; } = value;
    /// <summary>
    /// Whether the parameter doesn't have identifier specified.
    /// </summary>
    /// <remarks>
    /// Command can have a single nameless parameter.
    /// </remarks>
    public bool Nameless => Identifier is null;

    public Parameter (MixedValue value) : this(null, value) { }

    public override string ToString ()
    {
        var builder = new StringBuilder();
        if (!Nameless)
        {
            builder.Append(Identifier!);
            builder.Append(Identifiers.Default.ParameterAssign);
        }
        builder.Append(Value);
        return builder.ToString();
    }
}
