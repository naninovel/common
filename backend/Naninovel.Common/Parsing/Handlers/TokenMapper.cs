using System.Collections.Generic;

namespace Naninovel.Parsing;

/// <summary>
/// Allows mapping <see cref="Token"/> to associated <see cref="ILineComponent"/>.
/// </summary>
public class TokenMapper : IAssociator
{
    private readonly Dictionary<ILineComponent, Token> map = new();

    /// <summary>
    /// Maps provided token to the specified line component instance.
    /// </summary>
    public void Associate (ILineComponent component, Token token)
    {
        map[component] = token;
    }

    /// <summary>
    /// Attempts to resolve token mapped to the provided line component instance.
    /// </summary>
    /// <returns>Associated token or null.</returns>
    public Token? Resolve (ILineComponent component)
    {
        return map.TryGetValue(component, out var token) ? token : null;
    }

    /// <summary>
    /// Discards stored data.
    /// </summary>
    public void Clear ()
    {
        map.Clear();
    }
}
