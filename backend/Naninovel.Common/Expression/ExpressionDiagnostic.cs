namespace Naninovel.Expression;

/// <summary>
/// An diagnostic message associated with a subset of expression text.
/// </summary>
public readonly struct ExpressionDiagnostic (int index, int length, string message)
{
    /// <summary>
    /// First index of an expression text associated with the diagnostic.
    /// </summary>
    public readonly int Index = index;
    /// <summary>
    /// Length of an expression text associated with the diagnostic.
    /// </summary>
    public readonly int Length = length;
    /// <summary>
    /// Diagnostic message.
    /// </summary>
    public readonly string Message = message;
}
