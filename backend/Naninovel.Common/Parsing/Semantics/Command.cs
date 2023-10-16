using System.Text;

namespace Naninovel.Parsing;

/// <summary>
/// Represents an operation executed at script playback.
/// </summary>
/// <remarks>
/// This type represent command body, which is a part of
/// <see cref="CommandLine"/> and <see cref="InlinedCommand"/>.
/// </remarks>
public class Command : ILineComponent
{
    /// <summary>
    /// Unique identifier of the command.
    /// </summary>
    /// <remarks>
    /// Not case-sensitive.
    /// In v1 can be either alias or type name of the command implementation.
    /// </remarks>
    public PlainText Identifier { get; }
    /// <summary>
    /// Parameters of the command describing its behaviour.
    /// </summary>
    public IReadOnlyList<Parameter> Parameters { get; }

    public Command (PlainText identifier) : this(identifier, Array.Empty<Parameter>()) { }

    public Command (PlainText identifier, IReadOnlyList<Parameter> parameters)
    {
        Identifier = identifier;
        Parameters = parameters;
    }

    public override string ToString ()
    {
        var builder = new StringBuilder(Identifier.Text);
        foreach (var parameter in Parameters)
            builder.Append(' ').Append(parameter);
        return builder.ToString();
    }
}
