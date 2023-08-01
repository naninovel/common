using System.Linq;
using Naninovel.Parsing;

namespace Naninovel.Metadata;

/// <summary>
/// Allows resolving <see cref="Endpoint"/> from parsed commands.
/// </summary>
public class EndpointResolver
{
    private readonly NamedValueParser namedParser = new();
    private readonly MetadataProvider provider;

    public EndpointResolver (MetadataProvider provider)
    {
        this.provider = provider;
    }

    /// <summary>
    /// Builds constant expression for label component of endpoint parameter.
    /// </summary>
    /// <param name="paramId">ID of the parameter the expression is built for.</param>
    /// <remarks>
    /// Endpoints are expected to be named parameters where name component is script
    /// name and value is the label. The expression will first attempt to get script name
    /// from the parameter and fallback to currently edited script when it's not specified.
    /// </remarks>
    public static string BuildEndpointExpression (string paramId)
    {
        return $"Labels/{{:{paramId}[0]??$Script}}";
    }

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
        return param != null && param.ValueContext?.ElementAtOrDefault(1)?.SubType == BuildEndpointExpression(param.Id);
    }
}
