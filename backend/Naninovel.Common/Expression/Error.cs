namespace Naninovel.Expression;

/// <summary>
/// Exception thrown from Naninovel script expression processing internals.
/// </summary>
public class Error (string message) : Naninovel.Error(message);
