namespace Naninovel.Metadata;

/// <summary>
/// Provides efficient lookup over Naninovel project metadata.
/// </summary>
public class MetadataProvider
{
    public IReadOnlyCollection<Actor> Actors => actors;
    public IReadOnlyCollection<Command> Commands => commands;
    public IReadOnlyCollection<Constant> Constants => constants;
    public IReadOnlyCollection<Resource> Resources => resources;
    public IReadOnlyCollection<string> Variables => variables;
    public IReadOnlyCollection<string> Functions => functions;

    private readonly List<Actor> actors = [];
    private readonly List<Command> commands = [];
    private readonly List<Constant> constants = [];
    private readonly List<Resource> resources = [];
    private readonly List<string> variables = [];
    private readonly List<string> functions = [];
    private readonly Dictionary<string, Command> commandById = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Command> commandByAlias = new(StringComparer.OrdinalIgnoreCase);

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
    }

    /// <summary>
    /// Attempts to find a command, which has either specified ID or alias.
    /// Returns null when such command is not found.
    /// </summary>
    /// <param name="aliasOrId">Alias or ID of the command.</param>
    public Command? FindCommand (string aliasOrId)
    {
        if (commandByAlias.TryGetValue(aliasOrId, out var byAlias)) return byAlias;
        if (commandById.TryGetValue(aliasOrId, out var byId)) return byId;
        return null;
    }

    /// <summary>
    /// Attempts to find a parameter of a command, which has either specified ID or alias.
    /// Returns null when such parameter or command are not found.
    /// </summary>
    /// <param name="commandAliasOrId">Alias or ID of the parameter's command.</param>
    /// <param name="paramAliasOrId">Alias or ID of the parameter; use empty or null for nameless.</param>
    public Parameter? FindParameter (string commandAliasOrId, string paramAliasOrId)
    {
        return FindCommand(commandAliasOrId)?.Parameters
            .FirstOrDefault(p => p.Nameless && string.IsNullOrEmpty(paramAliasOrId) ||
                                 !string.IsNullOrEmpty(p.Alias) && string.Equals(p.Alias, paramAliasOrId, StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(p.Id, paramAliasOrId, StringComparison.OrdinalIgnoreCase));
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
