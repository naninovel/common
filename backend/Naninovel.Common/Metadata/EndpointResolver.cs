using Naninovel.Parsing;

namespace Naninovel.Metadata;

/// <summary>
/// Allows resolving <see cref="Endpoint"/> from parsed commands.
/// </summary>
public class EndpointResolver (MetadataProvider provider)
{
    private readonly NamedValueParser namedParser = new();

    /// <summary>
    /// When specified command has a parameter with navigation context (script name and/or label),
    /// returns true and assigns related out arguments; returns false otherwise.
    /// </summary>
    /// <param name="command">Command to extract the endpoint from.</param>
    /// <param name="endpoint">When found, assigns the endpoint; default otherwise.</param>
    /// <returns>Whether script name and/or label were found in one of the command parameters.</returns>
    public bool TryResolve (Parsing.Command command, out Endpoint endpoint)
    {
        foreach (var parameter in command.Parameters)
            if (TryResolve(parameter, command.Identifier, out endpoint))
                return true;
        endpoint = default;
        return false;
    }

    /// <summary>
    /// When specified parameter has navigation context (script name and/or label),
    /// returns true and assigns related out arguments; returns false otherwise.
    /// </summary>
    /// <param name="parameter">Parameter to extract the endpoint from.</param>
    /// <param name="commandAliasOrId">Identifier or alias of the command which the parameter is associated with.</param>
    /// <param name="endpoint">When found, assigns the endpoint; default otherwise.</param>
    /// <returns>Whether script name and/or label were found in one of the command parameters.</returns>
    public bool TryResolve (Parsing.Parameter parameter, string commandAliasOrId, out Endpoint endpoint)
    {
        if (HasEndpointContext(commandAliasOrId, parameter.Identifier))
        {
            var (script, label) = namedParser.Parse(parameter.Value.ToString());
            endpoint = new Endpoint(script, label);
            return true;
        }
        endpoint = default;
        return false;
    }

    private bool HasEndpointContext (string commandAliasOrId, string? paramAliasOrId)
    {
        var param = provider.FindParameter(commandAliasOrId, paramAliasOrId ?? "");
        if (param?.ValueContext is null ||
            param.ValueContext.Length != 2 ||
            param.ValueContext.Any(c => c is null)) return false;
        return param.ValueContext[0]!.Type == ValueContextType.Endpoint &&
               param.ValueContext[0]!.SubType == Constants.EndpointScript &&
               param.ValueContext[1]!.Type == ValueContextType.Endpoint &&
               param.ValueContext[1]!.SubType == Constants.EndpointLabel;
    }
}
