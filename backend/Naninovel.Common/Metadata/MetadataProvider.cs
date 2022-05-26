using System;
using System.Collections.Generic;
using System.Linq;

namespace Naninovel.Metadata;

/// <summary>
/// Provides efficient lookup over Naninovel project metadata.
/// </summary>
public class MetadataProvider
{
    public IReadOnlyCollection<Actor> Actors { get; }
    public IReadOnlyCollection<Command> Commands { get; }
    public IReadOnlyCollection<Constant> Constants { get; }
    public IReadOnlyCollection<Resource> Resources { get; }
    public IReadOnlyCollection<string> Variables { get; }
    public IReadOnlyCollection<string> Functions { get; }

    private readonly Dictionary<string, Command> commandById = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Command> commandByAlias = new(StringComparer.OrdinalIgnoreCase);

    public MetadataProvider (Project project)
    {
        Actors = project.Actors ?? Array.Empty<Actor>();
        Commands = project.Commands ?? Array.Empty<Command>();
        Constants = project.Constants ?? Array.Empty<Constant>();
        Resources = project.Resources ?? Array.Empty<Resource>();
        Variables = project.Variables ?? Array.Empty<string>();
        Functions = project.Functions ?? Array.Empty<string>();
        foreach (var command in Commands)
            IndexCommand(command);
    }

    /// <summary>
    /// Attempts to find a command, which has either specified ID or alias.
    /// Returns null when such command is not found.
    /// </summary>
    public Command FindCommand (string aliasOrId)
    {
        if (commandByAlias.TryGetValue(aliasOrId, out var byAlias)) return byAlias;
        if (commandById.TryGetValue(aliasOrId, out var byId)) return byId;
        return null;
    }

    /// <summary>
    /// Attempts to find a parameter of a command, which has either specified ID or alias.
    /// Returns null when such parameter or command are not found.
    /// </summary>
    public Parameter FindParameter (string commandAliasOrId, string paramAliasOrId)
    {
        return FindCommand(commandAliasOrId)?.Parameters?
            .FirstOrDefault(p => p.Nameless && paramAliasOrId == string.Empty ||
                                 string.Equals(p.Alias, paramAliasOrId, StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(p.Id, paramAliasOrId, StringComparison.OrdinalIgnoreCase));
    }

    private void IndexCommand (Command command)
    {
        commandById[command.Id] = command;
        if (!string.IsNullOrEmpty(command.Alias))
            commandByAlias[command.Alias] = command;
    }
}
