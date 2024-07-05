using System.Text;

/// <summary>
/// Allows splitting and joining text while escaping the separator.
/// </summary>
/// <remarks>
/// Un-/escaping (adding and removing of the specified escape character) will only happen
/// for specified seperator; escapes of the non-seperator characters will be ignored.
/// </remarks>
/// <param name="separator">The character to split and join with.</param>
/// <param name="escape">The character used to escape separator.</param>
public class TextSplitter (char separator, char escape = '\\')
{
    private readonly StringBuilder buffer = new();

    /// <summary>
    /// Splits specified string into substrings (entries) using specified character as separator
    /// and adds the split entries to specified collection. When the separator character is
    /// escaped, will not split, but un-escape the character.
    /// </summary>
    /// <remarks>
    /// Empty entries are added as empty strings.
    /// </remarks>
    /// <param name="str">The string to split.</param>
    /// <param name="entries">The collection to insert split entries.</param>
    public void Split (string str, ICollection<string> entries)
    {
        Reset();
        var escaped = false;
        for (var i = 0; i < str.Length; i++)
            if (!escaped && str[i] == separator) Separate();
            else if (!escaped && str[i] == escape) Escape(i);
            else AppendChar(str[i]);
        entries.Add(buffer.ToString());

        void Separate ()
        {
            entries.Add(buffer.ToString());
            buffer.Clear();
        }

        void Escape (int i)
        {
            escaped = true;
            var next = i + 1 < str.Length ? str[i + 1] : default;
            if (next != separator) buffer.Append(str[i]);
        }

        void AppendChar (char c)
        {
            buffer.Append(c);
            escaped = false;
        }
    }

    /// <summary>
    /// Joins specified string entries with specified separator character.
    /// Separator character will be escaped when found in the joined substrings.
    /// </summary>
    /// <param name="entries">The substrings to join.</param>
    /// <returns>The joined string.</returns>
    public string Join (IEnumerable<string> entries)
    {
        Reset();
        var escaped = $"\\{separator}";
        var added = false;
        foreach (var entry in entries)
        {
            if (added) buffer.Append(separator);
            buffer.Append(Escape(entry));
            added = true;
        }
        return buffer.ToString();

        string Escape (string entry)
        {
            if (entry.Contains(separator))
                return entry.Replace(separator.ToString(), escaped);
            return entry;
        }
    }

    private void Reset ()
    {
        buffer.Clear();
    }
}
