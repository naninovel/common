namespace Naninovel.Metadata;

/// <summary>
/// Describes nesting properties of a <see cref="Command"/>.
/// </summary>
public class Nest
{
    /// <summary>
    /// Whether the command requires nested commands and can't execute otherwise.
    /// </summary>
    public bool Required { get; set; }
}
