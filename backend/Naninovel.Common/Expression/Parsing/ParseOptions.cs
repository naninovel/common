using Naninovel.Parsing;

namespace Naninovel.Expression;

/// <summary>
/// Configures <see cref="Parser"/> instance.
/// </summary>
public class ParseOptions
{
    public Action<ParseDiagnostic>? HandleDiagnostic { get; set; }
    public Action<ExpressionRange>? HandleRange { get; set; }
    public ISyntax Syntax { get; set; } = Parsing.Syntax.Default;
}
