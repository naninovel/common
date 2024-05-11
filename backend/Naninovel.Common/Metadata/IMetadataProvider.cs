using Naninovel.Parsing;

namespace Naninovel.Metadata;

/// <summary>
/// Provides access to Naninovel project metadata.
/// </summary>
public interface IMetadataProvider
{
    /// <inheritdoc cref="Project.Actors"/>
    public IReadOnlyCollection<Actor> Actors { get; }
    /// <inheritdoc cref="Project.Commands"/>
    public IReadOnlyCollection<Command> Commands { get; }
    /// <inheritdoc cref="Project.Constants"/>
    public IReadOnlyCollection<Constant> Constants { get; }
    /// <inheritdoc cref="Project.Resources"/>
    public IReadOnlyCollection<Resource> Resources { get; }
    /// <inheritdoc cref="Project.Variables"/>
    public IReadOnlyCollection<string> Variables { get; }
    /// <inheritdoc cref="Project.Functions"/>
    public IReadOnlyCollection<string> Functions { get; }
    /// <inheritdoc cref="Project.Syntax"/>
    public ISyntax Syntax { get; }

    /// <summary>
    /// Attempts to find a command, which has either specified ID or alias.
    /// Returns null when such command is not found.
    /// </summary>
    /// <param name="aliasOrId">Alias or ID of the command.</param>
    public Command? FindCommand (string aliasOrId);
    /// <summary>
    /// Attempts to find a parameter of a command, which has either specified ID or alias.
    /// Returns null when such parameter or command are not found.
    /// </summary>
    /// <param name="commandAliasOrId">Alias or ID of the parameter's command.</param>
    /// <param name="paramAliasOrId">Alias or ID of the parameter; use empty or null for nameless.</param>
    public Parameter? FindParameter (string commandAliasOrId, string paramAliasOrId);
}
