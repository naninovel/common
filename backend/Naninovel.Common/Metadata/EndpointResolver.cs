using Naninovel.Parsing;

namespace Naninovel.Metadata;

/// <summary>
/// Allows resolving <see cref="Endpoint"/> from parsed commands with <see cref="BranchTraits.Endpoint"/> branch flag.
/// </summary>
public class EndpointResolver (IMetadata meta)
{
    private readonly NamedValueParser namedParser = new(meta.Syntax);
    private readonly ExpressionEvaluator eval = new(meta);

    /// <summary>
    /// When specified command has <see cref="BranchTraits.Endpoint"/> branch flag with
    /// <see cref="Branch.Endpoint"/> expression or a parameter with navigation context
    /// (script path and/or label), returns true and assigns related out arguments;
    /// returns false otherwise.
    /// </summary>
    /// <param name="command">Command to extract the endpoint from.</param>
    /// <param name="endpoint">When found, assigns the endpoint; default otherwise.</param>
    /// <returns>Whether command branches and endpoint was found in one of the command parameters.</returns>
    public bool TryResolve (Parsing.Command command, out Endpoint endpoint)
    {
        endpoint = default;
        if (meta.FindCommand(command.Identifier) is { Branch.Endpoint: { } exp })
            return ResolveFromExpression(exp, ref endpoint);
        foreach (var parameter in command.Parameters)
            if (TryResolve(parameter, command.Identifier, out endpoint))
                return true;
        return false;
    }

    /// <summary>
    /// When command with specified ID has <see cref="BranchTraits.Endpoint"/> branch flag and
    /// specified parameter has navigation context (script path and/or label),
    /// returns true and assigns related out arguments; returns false otherwise.
    /// </summary>
    /// <param name="parameter">Parameter to extract the endpoint from.</param>
    /// <param name="commandAliasOrId">Identifier or alias of the command which the parameter is associated with.</param>
    /// <param name="endpoint">When found, assigns the endpoint; default otherwise.</param>
    /// <returns>Whether parameter is an endpoint and associated command branches.</returns>
    public bool TryResolve (Parsing.Parameter parameter, string commandAliasOrId, out Endpoint endpoint)
    {
        endpoint = default;
        if (meta.FindCommand(commandAliasOrId) is not { Branch: { } branch } ||
            !branch.Traits.HasFlag(BranchTraits.Endpoint)) return false;
        if (HasEndpointContext(commandAliasOrId, parameter.Identifier))
        {
            var (script, label) = namedParser.Parse(parameter.Value.ToString());
            endpoint = new(script, label);
            return true;
        }
        return false;
    }

    private bool HasEndpointContext (string commandAliasOrId, string? paramAliasOrId)
    {
        var param = meta.FindParameter(commandAliasOrId, paramAliasOrId);
        if (param?.ValueContext is null ||
            param.ValueContext.Length != 2 ||
            param.ValueContext.Any(c => c is null)) return false;
        return param.ValueContext[0]!.Type == ValueContextType.Endpoint &&
               param.ValueContext[0]!.SubType == Constants.EndpointScript &&
               param.ValueContext[1]!.Type == ValueContextType.Endpoint &&
               param.ValueContext[1]!.SubType == Constants.EndpointLabel;
    }

    private bool ResolveFromExpression (string expression, ref Endpoint endpoint)
    {
        var result = eval.Evaluate(expression);
        if (string.IsNullOrEmpty(result)) return false;
        endpoint = new(result, null);
        return true;
    }
}
