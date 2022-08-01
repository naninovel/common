using static Naninovel.Parsing.Utilities;
using static Naninovel.Parsing.Identifiers;

namespace Naninovel.Parsing;

/// <summary>
/// Allows parsing named values where name and value components are delimited by a dot.
/// </summary>
public class NamedValueParser
{
    /// <summary>
    /// Splits decoded named value into name and value components.
    /// </summary>
    /// <param name="value">The decoded named value to split.</param>
    /// <returns>Name and value pair; each could be null when not assigned (skipped).</returns>
    public (string name, string value) Parse (string value)
    {
        if (string.IsNullOrEmpty(value)) return (null, null);
        var delimiterIndex = FindDelimiterIndex(value);
        if (delimiterIndex < 0) return (Unescape(value), null);
        var name = delimiterIndex == 0 ? null : value.Substring(0, delimiterIndex);
        var namedValue = delimiterIndex == value.Length - 1 ? null
            : value.Substring(delimiterIndex + 1, value.Length - (delimiterIndex + 1));
        return (Unescape(name), Unescape(namedValue));
    }

    private static string Unescape (string value) => UnescapeCharacter(value, NamedDelimiter);

    private static int FindDelimiterIndex (string value)
    {
        for (int i = 0; i < value.Length; i++)
            if (value[i] == NamedDelimiter[0] && !IsEscaped(value, i))
                return i;
        return -1;
    }
}
