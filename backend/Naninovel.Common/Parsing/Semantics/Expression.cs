namespace Naninovel.Parsing;

/// <summary>
/// Script expression evaluated at runtime.
/// </summary>
public class Expression : ILineComponent, IValueComponent
{
    /// <summary>
    /// The evaluated body of the expression.
    /// </summary>
    public PlainText Body { get; }

    public Expression (PlainText body)
    {
        Body = body;
    }

    public override string ToString ()
    {
        return $"{Identifiers.ExpressionOpen}{Body}{Identifiers.ExpressionClose}";
    }
}
