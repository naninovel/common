namespace Naninovel.Utilities; // Don't remove Utilities sub-space
// to prevent conflicts with common string utils in Unity runtime.
// Don't use this utils in Unity runtime, as this version has breaking changes, such as
// GetBefore/After returning empty string instead of null, which causes all sorts of shit.
// This can be consolidated after moving to v2 of runtime.

/// <summary>
/// Common text-related helpers and extensions.
/// </summary>
public static class TextUtils
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
        return str.IndexOf(match, comp) is var idx and >= 0 ? str[..idx] : "";
    }

    /// <summary>
    /// Attempts to extract content before the specified match (on last occurence); returns empty when not found.
    /// </summary>
    public static string GetBeforeLast (this string str, string match, StringComparison comp = StringComparison.Ordinal)
    {
        return str.LastIndexOf(match, comp) is var idx and >= 0 ? str[..idx] : "";
    }

    /// <summary>
    /// Attempts to extract content after the specified match (on last occurence); returns empty when not found.
    /// </summary>
    public static string GetAfter (this string str, string match, StringComparison comp = StringComparison.Ordinal)
    {
        var matchIdx = str.LastIndexOf(match, comp);
        if (matchIdx == -1) return "";
        var cutIdx = matchIdx + match.Length;
        return cutIdx < str.Length ? str[cutIdx..] : "";
    }

    /// <summary>
    /// Attempts to extract content after the specified match (on first occurence); returns empty when not found.
    /// </summary>
    public static string GetAfterFirst (this string str, string match, StringComparison comp = StringComparison.Ordinal)
    {
        var matchIdx = str.IndexOf(match, comp);
        if (matchIdx == -1) return "";
        var cutIdx = matchIdx + match.Length;
        return cutIdx < str.Length ? str[cutIdx..] : "";
    }

    /// <summary>
    /// Changes first character in the specified string to lower invariant.
    /// </summary>
    public static string FirstToLower (this string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsLower(str, 0)) return str;
        if (str.Length <= 1) return str.ToLowerInvariant();
        return $"{char.ToLowerInvariant(str[0])}{str[1..]}";
    }

    /// <summary>
    /// Changes first character in the specified string to upper invariant.
    /// </summary>
    public static string FirstToUpper (this string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsUpper(str, 0)) return str;
        if (str.Length <= 1) return str.ToUpperInvariant();
        return $"{char.ToUpperInvariant(str[0])}{str[1..]}";
    }

    /// <summary>
    /// Removes specified <paramref name="invalid"/> characters from the string.
    /// </summary>
    /// <remarks>
    /// This is faster than using <see cref="string.Replace(char,char)"/>, as it only allocates once
    /// and doesn't allocate at all (returns initial string) when the string doesn't contain invalid chars.
    /// </remarks>
    public static string Sanitize (this string str, IReadOnlyCollection<char> invalid)
    {
        // TODO: Use IReadOnlySet when Unity switches to the modern .NET.

        var validCharCount = 0;
        foreach (var c in str)
            if (!invalid.Contains(c))
                validCharCount++;
        if (validCharCount == str.Length) return str;

        return string.Create(validCharCount, (str, invalid), (span, ctx) => {
            var idx = 0;
            foreach (var c in ctx.str)
                if (!ctx.invalid.Contains(c))
                    span[idx++] = c;
        });
    }
}
