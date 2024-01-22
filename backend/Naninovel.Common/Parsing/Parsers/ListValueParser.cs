using static Naninovel.Parsing.Utilities;
using static Naninovel.Parsing.Identifiers;

namespace Naninovel.Parsing;

/// <summary>
/// Allows parsing list values where items are delimited by commas.
/// </summary>
public class ListValueParser
{
    private readonly List<string?> items = [];

    private int prevDelimiterIndex;
    private string value = "";

    /// <summary>
    /// Splits decoded list value into individual items.
    /// </summary>
    /// <param name="value">The decoded list value to split.</param>
    /// <returns>List of the items; each could be null when not assigned (skipped).</returns>
    public string?[] Parse (string value)
    {
        if (string.IsNullOrEmpty(value))
            return Array.Empty<string>();
        Reset(value);
        for (int i = 0; i < value.Length; i++)
            ProcessCharAt(i);
        items.Add(ExtractBeforeDelimiter(value.Length));
        return items.ToArray();
    }

    private void Reset (string value)
    {
        prevDelimiterIndex = -1;
        this.value = value;
        items.Clear();
    }

    private void ProcessCharAt (int index)
    {
        if (!IsDelimiter(index)) return;
        items.Add(ExtractBeforeDelimiter(index));
        prevDelimiterIndex = index;
    }

    private bool IsDelimiter (int i) => value[i] == ListDelimiter[0] && !IsEscaped(value, i);

    private string? ExtractBeforeDelimiter (int delimiterIndex)
    {
        var startIndex = prevDelimiterIndex + 1;
        var length = delimiterIndex - startIndex;
        if (length == 0) return null;
        var item = value.Substring(startIndex, length);
        return UnescapeCharacter(item, ListDelimiter);
    }
}
