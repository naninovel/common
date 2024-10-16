namespace Naninovel.Expression;

/// <summary>
/// Returns result or invoking function with specified name and parameters.
/// </summary>
public delegate IOperand ResolveFunction (string name, IReadOnlyList<IOperand> parameters);
