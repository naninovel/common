namespace Naninovel.Parsing;

/// <summary>
/// Handle associations between parsed line semantics
/// and script text line ranges used to represent them.
/// </summary>
public interface IRangeAssociator
{
    /// <summary>
    /// Handles association between the provided component and range.
    /// </summary>
    void Associate (ILineComponent component, LineRange range);
}
