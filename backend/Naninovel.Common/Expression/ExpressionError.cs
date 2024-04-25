namespace Naninovel.Expression;

/// <summary>
/// Exception thrown from Naninovel script expression processing internals.
/// </summary>
public class ExpressionError (string message) : Error(message);
