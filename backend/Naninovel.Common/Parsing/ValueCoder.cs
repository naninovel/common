using System;
using System.Collections.Generic;
using System.Linq;
using static Naninovel.Parsing.Identifiers;

namespace Naninovel.Parsing;

/// <summary>
/// Allows encoding and decoding command parameter values.
/// </summary>
public static class ValueCoder
{
    private static readonly char[] controlChars = {
        ExpressionOpen[0],
        ExpressionClose[0],
        InlinedOpen[0],
        InlinedClose[0],
        '\\'
    };

    /// <summary>
    /// Returns raw parameter value with unescaped control symbols
    /// and (optionally) unwrapped from quotes; expressions fragments are preserved as is.
    /// </summary>
    /// <param name="value">The value to decode.</param>
    public static string Decode (string value, bool unwrap = true)
    {
        if (string.IsNullOrEmpty(value)) return value;
        unwrap = unwrap && IsWrapped();
        if (unwrap) value = Unwrap();
        if (value.Length < 2) return value;
        return UnescapeMixed(value, unwrap);

        bool IsWrapped () => value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"';

        string Unwrap ()
        {
            if (value.Length == 2) return string.Empty;
            return value.Substring(1, value.Length - 2);
        }
    }

    internal static string UnescapeMixed (string value, bool unescapeQuotes)
    {
        var expression = false;
        for (int i = value.Length - 2; i >= 0; i--)
            if (TryExpression(i) || TryRemove(i))
                continue;
        return value;

        bool TryExpression (int i)
        {
            if (!IsEscaped(value, i) && value[i] == ExpressionClose[0])
                return expression = true;
            if (!IsEscaped(value, i) && value[i] == ExpressionOpen[0])
                return expression = false;
            return false;
        }

        bool TryRemove (int i)
        {
            if (!ShouldRemove(i)) return false;
            value = value.Remove(i, 1);
            return true;
        }

        bool ShouldRemove (int i)
        {
            if (expression || value[i] != '\\' || IsEscaped(value, i)) return false;
            var prevChar = value[i + 1];
            return unescapeQuotes && prevChar == '"' || controlChars.Contains(prevChar);
        }
    }

    /// <summary>
    /// Prepares a raw value to be used in script text by escaping control symbols
    /// and (optionally) wrapping in quotes when unwrapped whitespace is found.
    /// </summary>
    /// <param name="value">The value to encode.</param>
    /// <param name="ignoredRanges">The ranges to ignore when encoding (eg, expressions).</param>
    /// <param name="wrap">Whether to wrap the value in quotes when it contains whitespace.</param>
    public static string Encode (string value,
        IReadOnlyCollection<(int start, int length)> ignoredRanges = null, bool wrap = true)
    {
        if (string.IsNullOrEmpty(value)) return value;
        wrap = wrap && IsAnySpaceUnwrappedOrContainUnclosedQuotes();
        for (int i = value.Length - 1; i >= 0; i--)
            if (ShouldEscape(i))
                value = value.Insert(i, "\\");
        if (wrap) value = $"\"{value}\"";
        return value;

        bool IsAnySpaceUnwrappedOrContainUnclosedQuotes ()
        {
            bool wrapping = false, anySpace = false;
            for (int i = 0; i < value.Length; i++)
                if (IsIgnored(ignoredRanges, i)) continue;
                else if (!wrapping && char.IsWhiteSpace(value[i])) return true;
                else if (value[i] == '"' && !IsEscaped(value, i)) wrapping = !wrapping;
                else if (char.IsWhiteSpace(value[i])) anySpace = true;
            return wrapping && anySpace || wrapping;
        }

        bool ShouldEscape (int i)
        {
            if (IsIgnored(ignoredRanges, i)) return false;
            return controlChars.Contains(value[i]) || wrap && value[i] == '"';
        }
    }

    /// <summary>
    /// Escapes un-escaped quotes in the provided value and wraps it in quotes.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <param name="ignoredRanges">The ranges to ignore when wrapping (eg, expressions).</param>
    public static string Wrap (string value,
        IReadOnlyCollection<(int start, int length)> ignoredRanges = null)
    {
        if (value is null) return null;
        if (value == "") return "\"\"";
        for (int i = value.Length - 1; i >= 0; i--)
            if (ShouldEscape(i))
                value = value.Insert(i, "\\");
        return $"\"{value}\"";

        bool ShouldEscape (int i)
        {
            if (IsIgnored(ignoredRanges, i)) return false;
            return value[i] == '"' && !IsEscaped(value, i);
        }
    }

    /// <summary>
    /// Splits (decoded) list value into individual items.
    /// </summary>
    /// <param name="value">The decoded list value to split.</param>
    /// <returns>List of the items; each could be null when not assigned (skipped).</returns>
    public static IReadOnlyList<string> SplitList (string value)
    {
        if (string.IsNullOrEmpty(value)) return Array.Empty<string>();
        var list = new List<string>();
        var prevDelimiterIndex = -1;
        for (int i = 0; i < value.Length; i++)
            ProcessCharAt(i);
        list.Add(ExtractBeforeDelimiter(value.Length));
        return list;

        void ProcessCharAt (int index)
        {
            if (!IsDelimiter(index)) return;
            list.Add(ExtractBeforeDelimiter(index));
            prevDelimiterIndex = index;
        }

        bool IsDelimiter (int i) => value[i] == ListDelimiter[0] && !IsEscaped(value, i);

        string ExtractBeforeDelimiter (int delimiterIndex)
        {
            var startIndex = prevDelimiterIndex + 1;
            var length = delimiterIndex - startIndex;
            var item = length == 0 ? null : value.Substring(startIndex, length);
            return UnescapeCharacter(item, ListDelimiter);
        }
    }

    /// <summary>
    /// Splits (decoded) named value into name and value components.
    /// </summary>
    /// <param name="value">The decoded named value to split.</param>
    /// <returns>Name and value pair; each could be null when not assigned (skipped).</returns>
    public static (string name, string value) SplitNamed (string value)
    {
        if (string.IsNullOrEmpty(value)) return (null, null);
        var delimiterIndex = FindDelimiterIndex(value);
        if (delimiterIndex < 0) return (Unescape(value), null);
        var name = delimiterIndex == 0 ? null : value.Substring(0, delimiterIndex);
        var namedValue = delimiterIndex == value.Length - 1 ? null
            : value.Substring(delimiterIndex + 1, value.Length - (delimiterIndex + 1));
        return (Unescape(name), Unescape(namedValue));

        static string Unescape (string value) => UnescapeCharacter(value, NamedDelimiter);

        static int FindDelimiterIndex (string value)
        {
            for (int i = 0; i < value.Length; i++)
                if (value[i] == NamedDelimiter[0] && !IsEscaped(value, i))
                    return i;
            return -1;
        }
    }

    /// <summary>
    /// Tests whether char at the provided index inside specified value is escaped.
    /// </summary>
    public static bool IsEscaped (string value, int i)
    {
        return i > 0 && value[i - 1] == '\\' && !IsEscaped(value, i - 1);
    }

    private static bool IsIgnored (IEnumerable<(int start, int length)> ignoredRanges, int i)
    {
        if (ignoredRanges is null) return false;
        foreach (var (start, length) in ignoredRanges)
            if (i >= start && i < start + length)
                return true;
        return false;
    }

    private static string UnescapeCharacter (string content, string character)
    {
        if (content is null) return null;
        var escaped = "\\" + character;
        return content.Contains(escaped) ?
            content.Replace(escaped, character) : content;
    }
}
