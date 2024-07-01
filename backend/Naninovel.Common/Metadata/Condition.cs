using Naninovel.Parsing;

namespace Naninovel.Metadata;

/// <summary>
/// A condition for command execution resolved via <see cref="ConditionResolver"/>.
/// </summary>
public readonly struct Condition (IScriptLine line, Parsing.Command command,
    Parsing.Parameter? parameter = null, bool inverted = false) : IEquatable<Condition>
{
    /// <summary>
    /// Script line associated with the condition.
    /// </summary>
    public IScriptLine Line { get; } = line;
    /// <summary>
    /// Command associated with the condition.
    /// </summary>
    public Parsing.Command Command { get; } = command;
    /// <summary>
    /// Parameter associated with the condition, if any.
    /// </summary>
    public Parsing.Parameter? Parameter { get; } = parameter;
    /// <summary>
    /// Whether the condition is inverted, ie checked command execution will
    /// happen when the condition is falsy and vice-versa.
    /// </summary>
    public bool Inverted { get; } = inverted;

    public bool Equals (Condition other) =>
        Line.Equals(other.Line) &&
        Command.Equals(other.Command) &&
        Equals(Parameter, other.Parameter) &&
        Inverted == other.Inverted;

    public override bool Equals (object? obj) =>
        obj is Condition other &&
        Equals(other);

    public override int GetHashCode ()
    {
        unchecked
        {
            var hashCode = Line.GetHashCode();
            hashCode = (hashCode * 397) ^ Command.GetHashCode();
            hashCode = (hashCode * 397) ^ (Parameter != null ? Parameter.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Inverted.GetHashCode();
            return hashCode;
        }
    }
}
