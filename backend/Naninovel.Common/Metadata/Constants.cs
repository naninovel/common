namespace Naninovel.Metadata;

/// <summary>
/// Pre-defined constants of any Naninovel project.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Default type of the character actors.
    /// </summary>
    public const string CharacterType = "Characters";
    /// <summary>
    /// Default type of the background actors.
    /// </summary>
    public const string BackgroundType = "Backgrounds";
    /// <summary>
    /// Flag representing any type.
    /// </summary>
    public const string WildcardType = "*";
    /// <summary>
    /// Name expression assigned to named value of constant parameter context to
    /// identify label component of endpoint resolved via <see cref="EndpointResolver"/>.
    /// </summary>
    public const string LabelExpression = "Labels/{:Path[0]??$Script}";
}
