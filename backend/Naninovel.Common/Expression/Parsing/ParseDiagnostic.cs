namespace Naninovel.Expression;

/// <summary>
/// A diagnostic message associated with parsing a subset of expression text.
/// </summary>
public readonly struct ParseDiagnostic (int index, int length, string message) : IEquatable<ParseDiagnostic>
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

    public bool Equals (ParseDiagnostic other)
    {
        return Index == other.Index &&
               Length == other.Length &&
               Message == other.Message;
    }
}
