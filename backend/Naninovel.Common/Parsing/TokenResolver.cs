using System.Collections.Generic;

namespace Naninovel.Parsing;

/// <summary>
/// Allows keeping track of <see cref="Token"/> associated with <see cref="ILineComponent"/>.
/// </summary>
public class TokenResolver
{
    private readonly Dictionary<ILineComponent, Token> map = new();

    /// <summary>
    /// Associates provided line component instance with the specified token.
    /// </summary>
    public void Associate (ILineComponent component, Token token)
    {
        map[component] = token;
    }

    /// <summary>
    /// Attempts to resolve token associated with the provided line component instance.
    /// </summary>
    /// <returns>Associated token or null.</returns>
    public Token? Resolve (ILineComponent component)
    {
        return map.TryGetValue(component, out var token) ? token : null;
    }

    /// <summary>
    /// Discards stored associations.
    /// </summary>
    public void Clear ()
    {
        map.Clear();
    }
}
