using Naninovel.Parsing;

namespace Naninovel.Metadata;

/// <summary>
/// Allows resolving conditions for <see cref="Command"/> execution.
/// </summary>
public class ConditionResolver (IMetadata meta)
{
    private readonly HashSet<Parsing.Command> extractedCommands = [];
    private IList<Condition> conditions = null!;
    private Parsing.Command selfCommand = null!;
    private IScriptLine? selfLine;
    private bool conditionAdded;
    private string? switchRoot;
    private int minIndent = -1;

    /// <summary>
    /// Attempts to resolve execution conditions for specified command in the context of specified script.
    /// Will check condition of the command itself, as well as nested host (eg, nested @if or @choice blocks)
    /// in case the command is nested.
    /// </summary>
    /// <param name="command">The command for which to resolve conditions.</param>
    /// <param name="script">Script lines. One of the lines is expected to contain specified command.</param>
    /// <param name="conditions">The collection to which append resolved conditions.</param>
    /// <returns>Whether any condition was resolved and added to the specified collection.</returns>
    /// <exception cref="Error">Specified command not found in the specified script lines.</exception>
    public bool TryResolve (Parsing.Command command, IReadOnlyList<IScriptLine> script, IList<Condition> conditions)
    {
        Reset(command, conditions);
        for (int i = script.Count - 1; i >= 0; i--)
            if (!Move(script[i]))
                break;
        if (selfLine is null) throw new Error("Failed to resolve condition: specified command not found in script.");
        return conditionAdded;
    }

    /// <summary>
    /// Attempts to find a parameter with <see cref="ValueContextType.Expression"/> context and
    /// <see cref="Constants.Condition"/> sub-type in the specified command.
    /// </summary>
    /// <param name="command">Command in which to look for condition parameter.</param>
    /// <param name="param">Found parameter or null.</param>
    /// <returns>Whether condition parameter is found.</returns>
    public bool TryResolve (Parsing.Command command, out Parsing.Parameter param) =>
        (param = command.Parameters.FirstOrDefault(p => IsCondition(p.Identifier, command.Identifier))) != null;

    /// <summary>
    /// Checks whether specified parameter of specified command is a condition, ie has
    /// <see cref="ValueContextType.Expression"/> context and <see cref="Constants.Condition"/> sub-type
    /// and has a value assigned.
    /// </summary>
    /// <param name="paramAliasOrId">Identifier or alias of the parameter to check.</param>
    /// <param name="commandAliasOrId">Identifier or alias of the command containing the checked parameter.</param>
    public bool IsCondition (string paramAliasOrId, string commandAliasOrId) =>
        meta.FindParameter(commandAliasOrId, paramAliasOrId)?.ValueContext?.FirstOrDefault()
            is { Type: ValueContextType.Expression, SubType: Constants.Condition };

    private void Reset (Parsing.Command command, IList<Condition> conditions)
    {
        this.conditions = conditions;
        selfCommand = command;
        selfLine = null;
        switchRoot = null;
        conditionAdded = false;
        minIndent = int.MaxValue;
    }

    private bool Move (IScriptLine line)
    {
        var commandsInLine = ExtractCommands(line);
        selfLine ??= commandsInLine.Contains(selfCommand) ? line : null;
        if (selfLine is null) return true;
        if (selfLine == line)
        {
            if (TryResolve(selfCommand, out var selfParam))
                AddCondition(selfLine, selfCommand, selfParam);
            minIndent = line.Indent;
            return selfLine.Indent > 0;
        }
        if (line.Indent > minIndent) return true;
        if (line is not CommandLine { Command: { } lineCommand }) return true;
        if (meta.FindCommand(lineCommand.Identifier) is not { Branch: { } branch } cmdMeta) return true;
        if (!branch.Traits.HasFlag(BranchTraits.Nest)) return true;
        if (TryResolve(lineCommand, out var param) || branch.Traits.HasFlag(BranchTraits.Interactive))
        {
            if (line.Indent == minIndent)
            {
                if (!string.IsNullOrEmpty(switchRoot) && (switchRoot == branch.SwitchRoot || switchRoot == cmdMeta.Id))
                    AddCondition(line, lineCommand, param, true);
                if (branch.Traits.HasFlag(BranchTraits.Switch))
                    switchRoot = null;
            }
            else
            {
                AddCondition(line, lineCommand, param);
                switchRoot = branch.SwitchRoot;
            }
        }
        if (line.Indent < minIndent)
        {
            minIndent = line.Indent;
            switchRoot = branch.SwitchRoot;
        }
        return true;
    }

    private void AddCondition (IScriptLine line, Parsing.Command command, Parsing.Parameter? param, bool inverse = false)
    {
        conditions.Add(new(line, command, param, inverse));
        conditionAdded = true;
    }

    private IReadOnlyCollection<Parsing.Command> ExtractCommands (IScriptLine line)
    {
        extractedCommands.Clear();
        if (line is CommandLine commandLine)
            extractedCommands.Add(commandLine.Command);
        else if (line is GenericLine genericLine)
            foreach (var content in genericLine.Content)
                if (content is InlinedCommand inlined)
                    extractedCommands.Add(inlined.Command);
        return extractedCommands;
    }
}
