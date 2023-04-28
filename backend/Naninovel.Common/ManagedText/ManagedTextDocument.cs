using System.Collections.Generic;

namespace Naninovel.ManagedText;

/// <summary>
/// An immutable collection of <see cref="ManagedTextRecord"/> with unique keys.
/// </summary>
public class ManagedTextDocument
{
    public IReadOnlyCollection<ManagedTextRecord> Records => map.Values;

    private readonly Dictionary<string, ManagedTextRecord> map = new();

    public ManagedTextDocument (IEnumerable<ManagedTextRecord> records)
    {
        foreach (var record in records)
            map[record.Key] = record;
    }

    public bool Contains (string key) => map.ContainsKey(key);
    public ManagedTextRecord Get (string key) => map[key];
    public bool TryGet (string key, out ManagedTextRecord record) => map.TryGetValue(key, out record);
}
