namespace Naninovel.Parsing;

/// <summary>
/// Script expression evaluated at runtime.
/// </summary>
public class Expression : IMixedValue
{
    /// <summary>
    /// The evaluated content of the expression.
    /// </summary>
    public string Text { get; }

    public Expression (string text)
    {
        Text = text;
    }

    public override string ToString ()
    {
        return $"{Identifiers.ExpressionOpen}{Text}{Identifiers.ExpressionClose}";
    }
}
