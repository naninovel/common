namespace Naninovel.Parsing;

/// <summary>
/// Script expression evaluated at runtime.
/// </summary>
public class Expression (PlainText body) : ILineComponent, IValueComponent
{
    /// <summary>
    /// The evaluated body of the expression.
    /// </summary>
    public PlainText Body { get; } = body;

    public override string ToString ()
    {
        return $"{Identifiers.Default.ExpressionOpen}{Body}{Identifiers.Default.ExpressionClose}";
    }
}
