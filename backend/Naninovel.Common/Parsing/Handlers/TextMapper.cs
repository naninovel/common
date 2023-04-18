using System.Collections.Generic;

namespace Naninovel.Parsing;

/// <summary>
/// Maps <see cref="IdentifiedText.Id"/> to <see cref="IdentifiedText.Text"/>.
/// </summary>
public class TextMapper : ITextIdentifier
{
    /// <summary>
    /// Mapped text.
    /// </summary>
    public IReadOnlyDictionary<string, string> Map => map;

    private readonly Dictionary<string, string> map = new();

    public void Identify (string id, string text)
    {
        map[id] = text;
    }

    public void Clear ()
    {
        map.Clear();
    }
}
