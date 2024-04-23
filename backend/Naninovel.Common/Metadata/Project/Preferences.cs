using Naninovel.Parsing;

namespace Naninovel.Metadata;

/// <summary>
/// Various developer preferences.
/// </summary>
public class Preferences
{
    /// <summary>
    /// Identifier of the command feeding the generic text line parameters.
    /// </summary>
    public string ParametrizeGenericCommandId { get; set; } = "";
    /// <summary>
    /// Control symbols identifying NaniScript lexical artifacts.
    /// </summary>
    public Identifiers Identifiers { get; set; } = new();
}
