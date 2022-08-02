using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Naninovel.Parsing;

/// <summary>
/// Represents a collection of <see cref="IValueComponent"/>.
/// </summary>
public class MixedValue : IReadOnlyList<IValueComponent>, ILineComponent, IGenericContent
{
    /// <summary>
    /// Whether the value contains an <see cref="Expression"/> and will be evaluated at runtime.
    /// </summary>
    public bool Dynamic => HasExpression();
    public int Count => components.Count;

    private readonly IReadOnlyList<IValueComponent> components;

    public MixedValue (IEnumerable<IValueComponent> components)
    {
        this.components = components.ToArray();
    }

    public IValueComponent this [int index] => components[index];

    public IEnumerator<IValueComponent> GetEnumerator ()
    {
        return components.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
        return GetEnumerator();
    }

    public override string ToString ()
    {
        var builder = new StringBuilder();
        foreach (var component in components)
            builder.Append(component);
        return builder.ToString();
    }

    private bool HasExpression ()
    {
        foreach (var component in components)
            if (component is Expression)
                return true;
        return false;
    }
}
