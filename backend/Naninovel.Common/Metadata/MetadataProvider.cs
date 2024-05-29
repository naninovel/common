using Naninovel.Parsing;

namespace Naninovel.Metadata;

/// <summary>
/// Hosts and provides access to current Naninovel project metadata.
/// </summary>
public class MetadataProvider : IMetadata
{
    public IReadOnlyCollection<Actor> Actors => actors;
    public IReadOnlyCollection<Command> Commands => commands;
    public IReadOnlyCollection<Constant> Constants => constants;
    public IReadOnlyCollection<Resource> Resources => resources;
    public IReadOnlyCollection<string> Variables => variables;
    public IReadOnlyCollection<Function> Functions => functions;
    public ISyntax Syntax => syntaxProvider;

    private readonly List<Actor> actors = [];
    private readonly List<Command> commands = [];
    private readonly List<Constant> constants = [];
    private readonly List<Resource> resources = [];
    private readonly List<string> variables = [];
    private readonly List<Function> functions = [];
    private readonly Dictionary<string, Command> commandById = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Command> commandByAlias = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Function> functionByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly SyntaxProvider syntaxProvider = new();

    public MetadataProvider () { }
    public MetadataProvider (Project meta) => Update(meta);

    /// <summary>
    /// Replaces the current metadata.
    /// </summary>
    public void Update (Project meta)
    {
        Reset();
        actors.AddRange(meta.Actors);
        commands.AddRange(meta.Commands);
        constants.AddRange(meta.Constants);
        resources.AddRange(meta.Resources);
        variables.AddRange(meta.Variables);
        functions.AddRange(meta.Functions);
        foreach (var command in Commands)
            IndexCommand(command);
        foreach (var fn in Functions)
            functionByName[fn.Name] = fn;
        syntaxProvider.Update(meta.Syntax);
    }

    public Command? FindCommand (string aliasOrId)
    {
        if (commandByAlias.TryGetValue(aliasOrId, out var byAlias)) return byAlias;
        if (commandById.TryGetValue(aliasOrId, out var byId)) return byId;
        return null;
    }

    public Parameter? FindParameter (string commandAliasOrId, string paramAliasOrId)
    {
        return FindCommand(commandAliasOrId)?.Parameters.FirstOrDefault(IsMatch);

        bool IsMatch (Parameter p)
        {
            if (p.Nameless && string.IsNullOrEmpty(paramAliasOrId)) return true;
            if (!string.IsNullOrEmpty(p.Alias) && string.Equals(p.Alias, paramAliasOrId, StringComparison.OrdinalIgnoreCase)) return true;
            return string.Equals(p.Id, paramAliasOrId, StringComparison.OrdinalIgnoreCase);
        }
    }

    public Function? FindFunction (string name)
    {
        return functionByName.TryGetValue(name, out var fn) ? fn : null;
    }

    public FunctionParameter? FindFunctionParameter (string functionName, string paramName)
    {
        var fn = FindFunction(functionName);
        if (fn is null) return null;
        foreach (var param in fn.Parameters)
            if (string.Equals(param.Name, paramName, StringComparison.OrdinalIgnoreCase))
                return param;
        return null;
    }

    public FunctionParameter? FindFunctionParameter (string functionName, int index)
    {
        return FindFunction(functionName)?.Parameters.ElementAtOrDefault(index);
    }

    private void Reset ()
    {
        actors.Clear();
        commands.Clear();
        constants.Clear();
        resources.Clear();
        variables.Clear();
        functions.Clear();
        commandById.Clear();
        commandByAlias.Clear();
        functionByName.Clear();
    }

    private void IndexCommand (Command command)
    {
        commandById[command.Id] = command;
        if (!string.IsNullOrEmpty(command.Alias))
            commandByAlias[command.Alias!] = command;
    }
}
