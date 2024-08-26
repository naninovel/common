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
        syntaxProvider.Update(meta.Syntax);
    }

    public Command? FindCommand (string aliasOrId)
    {
        if (commandByAlias.TryGetValue(aliasOrId, out var byAlias)) return byAlias;
        return commandById.GetValueOrDefault(aliasOrId);
    }

    public Parameter? FindParameter (string commandAliasOrId, string? paramAliasOrId)
    {
        return FindCommand(commandAliasOrId)?.Parameters.FirstOrDefault(IsMatch);

        bool IsMatch (Parameter p)
        {
            if (p.Nameless && string.IsNullOrEmpty(paramAliasOrId)) return true;
            if (!string.IsNullOrEmpty(p.Alias) && string.Equals(p.Alias, paramAliasOrId, StringComparison.OrdinalIgnoreCase)) return true;
            return string.Equals(p.Id, paramAliasOrId, StringComparison.OrdinalIgnoreCase);
        }
    }

    public bool FindFunctions (string name, ICollection<Function> result)
    {
        result.Clear();
        foreach (var fn in functions)
            if (fn.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                result.Add(fn);
        return result.Count > 0;
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
    }

    private void IndexCommand (Command command)
    {
        commandById[command.Id] = command;
        if (!string.IsNullOrEmpty(command.Alias))
            commandByAlias[command.Alias!] = command;
    }
}
