using System.Collections;
using System.Collections.Generic;

namespace Naninovel.Parsing;

/// <summary>
/// Allows mapping <see cref="ILineComponent"/> to associated <see cref="InlineRange"/>.
/// </summary>
public class RangeMapper : IRangeAssociator, IEnumerable<KeyValuePair<ILineComponent, InlineRange>>
{
    private readonly Dictionary<ILineComponent, InlineRange> map = new();

    /// <summary>
    /// Maps provided component instance to the associated range.
    /// </summary>
    public void Associate (ILineComponent component, InlineRange range)
    {
        map[component] = range;
    }

    /// <summary>
    /// Attempts to resolve a range associated with the provided component instance.
    /// </summary>
    public bool TryResolve (ILineComponent component, out InlineRange range)
    {
        return map.TryGetValue(component, out range);
    }

    /// <summary>
    /// Discards stored data.
    /// </summary>
    public void Clear ()
    {
        map.Clear();
    }

    public IEnumerator<KeyValuePair<ILineComponent, InlineRange>> GetEnumerator ()
    {
        return map.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
        return GetEnumerator();
    }
}
