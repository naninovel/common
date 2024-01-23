namespace Naninovel.ManagedText;

/// <summary>
/// An immutable collection of <see cref="ManagedTextRecord"/> with unique keys.
/// </summary>
public class ManagedTextDocument
{
    /// <summary>
    /// Optional remarks/description associated with the document or empty.
    /// </summary>
    public string Header { get; }
    /// <summary>
    /// Records contained by the document or empty.
    /// </summary>
    public IReadOnlyCollection<ManagedTextRecord> Records => map.Values;

    private readonly Dictionary<string, ManagedTextRecord> map = new();

    public ManagedTextDocument (IEnumerable<ManagedTextRecord> records, string? header = null)
    {
        Header = header ?? "";
        foreach (var record in records)
            map[record.Key] = record;
    }

    public bool Contains (string key) => map.ContainsKey(key);
    public ManagedTextRecord Get (string key) => map[key];
    public bool TryGet (string key, out ManagedTextRecord record) => map.TryGetValue(key, out record);
}
