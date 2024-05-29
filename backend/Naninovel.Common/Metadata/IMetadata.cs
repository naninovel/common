using Naninovel.Parsing;

namespace Naninovel.Metadata;

/// <summary>
/// Provides access to current Naninovel project metadata.
/// </summary>
public interface IMetadata
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
    public IReadOnlyCollection<Function> Functions { get; }
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
    /// <summary>
    /// Attempts to find a function, which has specified name.
    /// Returns null when such function is not found.
    /// </summary>
    /// <param name="name">Name of the function.</param>
    public Function? FindFunction (string name);
    /// <summary>
    /// Attempts to find a parameter of a function, which has specified name.
    /// Returns null when such parameter or function are not found.
    /// </summary>
    /// <param name="functionName">Name of the parameter's function.</param>
    /// <param name="paramName">Name of the parameter.</param>
    public FunctionParameter? FindFunctionParameter (string functionName, string paramName);
    /// <summary>
    /// Attempts to find a parameter of a function, which has specified index in function signature.
    /// Returns null when such parameter or function are not found.
    /// </summary>
    /// <param name="functionName">Name of the parameter's function.</param>
    /// <param name="index">Index of the parameter in function signature.</param>
    public FunctionParameter? FindFunctionParameter (string functionName, int index);
}
