namespace Naninovel.Parsing;

/// <summary>
/// Script expression evaluated at runtime.
/// </summary>
public class Expression (PlainText body) : IValueComponent
{
    /// <summary>
    /// The evaluated body of the expression.
    /// </summary>
    public PlainText Body { get; } = body;

    public override string ToString ()
    {
        return $"{Syntax.Default.ExpressionOpen}{Body}{Syntax.Default.ExpressionClose}";
    }
}
