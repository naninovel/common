namespace Naninovel.Utilities;

/// <summary>
/// Common text-related helpers and extensions.
/// </summary>
public static class TextUtilities
{
    private static readonly string[] breaks = ["\r\n", "\n", "\r"];
    private static readonly char[] junk = ['\uFEFF', '\u200B'];

    /// <summary>
    /// Splits the string with line break symbol as separator.
    /// Will split by any type of line break, independent of OS.
    /// </summary>
    public static string[] SplitLines (this string str, StringSplitOptions options = StringSplitOptions.None)
    {
        return str.Split(breaks, options);
    }

    /// <summary>
    /// Returns iterator over the lines in the string.
    /// Will detect any type of line breaks, independent of OS.
    /// </summary>
    public static IEnumerable<string> IterateLines (this string str)
    {
        using var reader = new StringReader(str);
        while (reader.ReadLine() is { } line)
            yield return line;
    }

    /// <summary>
    /// Returns indexed iterator over the lines in the string.
    /// Will detect any type of line breaks, independent of OS.
    /// </summary>
    public static IEnumerable<(string line, int index)> IterateLinesIndexed (this string str)
    {
        using var reader = new StringReader(str);
        var index = -1;
        while (reader.ReadLine() is { } line)
            yield return (line, ++index);
    }

    /// <summary>
    /// Trims BOM, zero-width and other junk not removed with normal <see cref="string.Trim()"/>.
    /// </summary>
    public static string TrimJunk (this string str)
    {
        return str.Trim(junk);
    }

    /// <summary>
    /// Performs <see cref="string.EndsWith(string, StringComparison)"/> with <see cref="StringComparison.Ordinal"/>.
    /// </summary>
    public static bool EndsWithOrdinal (this string str, string match)
    {
        return str.EndsWith(match, StringComparison.Ordinal);
    }

    /// <summary>
    /// Performs <see cref="string.StartsWith(string, StringComparison)"/> with <see cref="StringComparison.Ordinal"/>.
    /// </summary>
    public static bool StartsWithOrdinal (this string str, string match)
    {
        return str.StartsWith(match, StringComparison.Ordinal);
    }

    /// <summary>
    /// Attempts to extract content before the specified match (on first occurence); returns empty when not found.
    /// </summary>
    public static string GetBefore (this string str, string match, StringComparison comp = StringComparison.Ordinal)
    {
        return str.IndexOf(match, comp) is var idx and >= 0 ? str.Substring(0, idx) : "";
    }

    /// <summary>
    /// Attempts to extract content before the specified match (on last occurence); returns empty when not found.
    /// </summary>
    public static string GetBeforeLast (this string str, string match, StringComparison comp = StringComparison.Ordinal)
    {
        return str.LastIndexOf(match, comp) is var idx and >= 0 ? str.Substring(0, idx) : "";
    }

    /// <summary>
    /// Attempts to extract content after the specified match (on last occurence); returns empty when not found.
    /// </summary>
    public static string GetAfter (this string str, string match, StringComparison comp = StringComparison.Ordinal)
    {
        return (str.LastIndexOf(match, comp) + match.Length) is var idx and >= 0 && idx < str.Length ? str.Substring(idx) : "";
    }

    /// <summary>
    /// Attempts to extract content after the specified match (on first occurence); returns empty when not found.
    /// </summary>
    public static string GetAfterFirst (this string str, string match, StringComparison comp = StringComparison.Ordinal)
    {
        return (str.IndexOf(match, comp) + match.Length) is var idx and >= 0 && idx < str.Length ? str.Substring(idx) : "";
    }

    /// <summary>
    /// Changes first character in the specified string to lower invariant.
    /// </summary>
    public static string FirstToLower (this string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsLower(str, 0)) return str;
        if (str.Length <= 1) return str.ToLowerInvariant();
        return $"{char.ToLowerInvariant(str[0])}{str.Substring(1)}";
    }

    /// <summary>
    /// Changes first character in the specified string to upper invariant.
    /// </summary>
    public static string FirstToUpper (this string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsUpper(str, 0)) return str;
        if (str.Length <= 1) return str.ToUpperInvariant();
        return $"{char.ToUpperInvariant(str[0])}{str.Substring(1)}";
    }
}
