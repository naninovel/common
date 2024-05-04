using Naninovel.Parsing;

namespace Naninovel.Expression;

/// <summary>
/// Configures <see cref="Parser"/> instance.
/// </summary>
public class ParseOptions
{
    public Action<ParseDiagnostic>? HandleDiagnostic { get; set; }
    public Identifiers Identifiers { get; set; } = Identifiers.Default;
}
