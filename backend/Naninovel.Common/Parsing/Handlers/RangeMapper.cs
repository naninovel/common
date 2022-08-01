using System.Collections.Generic;

namespace Naninovel.Parsing;

/// <summary>
/// Allows mapping <see cref="ILineComponent"/> to associated <see cref="LineRange"/>.
/// </summary>
public class RangeMapper : IAssociator
{
    private readonly Dictionary<ILineComponent, LineRange> map = new();

    /// <summary>
    /// Maps provided component instance to the associated range.
    /// </summary>
    public void Associate (ILineComponent component, LineRange range)
    {
        map[component] = range;
    }

    /// <summary>
    /// Attempts to resolve a range associated with the provided component instance.
    /// </summary>
    /// <returns>Associated range or null.</returns>
    public LineRange? Resolve (ILineComponent component)
    {
        return map.TryGetValue(component, out var range) ? range : null;
    }

    /// <summary>
    /// Discards stored data.
    /// </summary>
    public void Clear ()
    {
        map.Clear();
    }
}
