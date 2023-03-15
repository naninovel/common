using System.Linq;
using Naninovel.Parsing;

namespace Naninovel.Metadata;

/// <summary>
/// Allows resolving navigation endpoint (script name and/or label) from parsed commands.
/// </summary>
public class EndpointResolver
{
    private const string labelExpression = "Labels/{:Path[0]??$Script}";
    private readonly NamedValueParser namedParser = new();
    private readonly MetadataProvider provider;

    public EndpointResolver (MetadataProvider provider)
    {
        this.provider = provider;
    }

    /// <summary>
    /// When specified command has a parameter with navigation context (script name and/or label),
    /// returns true and assigns related out arguments; returns false otherwise.
    /// </summary>
    /// <param name="command">Command to extract the endpoint from.</param>
    /// <param name="script">When found, assigns script name to the argument; null otherwise.</param>
    /// <param name="label">When found, assigns label to the argument; null otherwise.</param>
    /// <returns>Whether script name and/or label were found in one of the command parameters.</returns>
    public bool TryResolve (Parsing.Command command, out string? script, out string? label)
    {
        script = null;
        label = null;

        foreach (var parameter in command.Parameters)
            if (IsScriptAndLabelValue(parameter))
            {
                (script, label) = namedParser.Parse(parameter.Value.ToString());
                return true;
            }

        return false;

        bool IsScriptAndLabelValue (Parsing.Parameter parameter) =>
            provider.FindParameter(command.Identifier.Text, parameter.Identifier?.Text ?? "")
                ?.ValueContext?.ElementAtOrDefault(1)?.SubType == labelExpression;
    }
}
