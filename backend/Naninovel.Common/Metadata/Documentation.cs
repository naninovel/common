namespace Naninovel.Metadata;

/// <summary>
/// Helpful information for end-user associated with a metadata entity.
/// </summary>
public class Documentation
{
    /// <summary>
    /// Human-readable description.
    /// </summary>
    public string? Summary { get; set; }
    /// <summary>
    /// Additional, less important information.
    /// </summary>
    public string? Remarks { get; set; }
    /// <summary>
    /// Usage examples.
    /// </summary>
    public string? Examples { get; set; }
}
