using System.Collections;
using System.Collections.Generic;

namespace Naninovel.Parsing;

/// <summary>
/// Allows mapping <see cref="ILineComponent"/> to associated <see cref="LineRange"/>.
/// </summary>
public class RangeMapper : IAssociator, IEnumerable<KeyValuePair<ILineComponent, LineRange>>
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
    public bool TryResolve (ILineComponent component, out LineRange range)
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

    public IEnumerator<KeyValuePair<ILineComponent, LineRange>> GetEnumerator ()
    {
        return map.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
        return GetEnumerator();
    }
}
