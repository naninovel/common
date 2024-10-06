namespace Naninovel;

/// <summary>
/// Common text-related helpers and extensions.
/// </summary>
public static class TextUtils
{
    /// <summary>
    /// Formats specified string for human readability by removing non-letter and
    /// non-digit characters and converting from snake_case, PascalCase, camelCase,
    /// kebab-case or SCREAMING_CASE to Title Case.
    /// </summary>
    public static string Humanize (this string str)
    {
        int idx, length = 0;
        char curr, prev, next, last = default;
        Span<char> buffer = stackalloc char[str.Length * 2];

        for (idx = 0; idx < str.Length; idx++)
        {
            curr = str[idx];
            prev = idx > 0 ? str[idx - 1] : default;
            next = idx + 1 < str.Length ? str[idx + 1] : default;
            last = length > 0 ? buffer[length - 1] : default;
            if (!Skip()) buffer[length++] = Upper() ? char.ToUpper(curr) : Lower() ? char.ToLower(curr) : curr;
            if (Space()) buffer[length++] = ' ';
        }

        return buffer[..length].ToString();

        bool Skip () => !char.IsLetterOrDigit(curr);
        bool Upper () => length == 1 || char.IsWhiteSpace(last);
        bool Lower () => char.IsUpper(curr) && char.IsUpper(prev);
        bool Space () => length > 0 && (
            char.IsLower(curr) && char.IsUpper(next) ||
            char.IsLetter(curr) && char.IsDigit(next) ||
            char.IsDigit(curr) && char.IsLetter(next) ||
            !char.IsLetterOrDigit(curr) && char.IsLetterOrDigit(next));
    }

    /// <summary>
    /// Converts specified string to PascalCase.
    /// </summary>
    public static string ToPascalCase (this string str)
    {
        int idx, length = 0;
        char curr, prev = default;
        Span<char> buffer = stackalloc char[str.Length];

        for (idx = 0; idx < str.Length; idx++)
        {
            curr = str[idx];
            prev = idx > 0 ? str[idx - 1] : default;
            if (!Skip()) buffer[length++] = Upper() ? char.ToUpper(curr) : Lower() ? char.ToLower(curr) : curr;
        }

        return buffer[..length].ToString();

        bool Skip () => !char.IsLetterOrDigit(curr);
        bool Upper () => length == 1 || !char.IsLetterOrDigit(prev);
        bool Lower () => char.IsUpper(curr) && char.IsUpper(prev);
    }

    /// <summary>
    /// Converts specified string to kebab-case.
    /// </summary>
    public static string ToKebabCase (this string str)
    {
        int idx, length = 0;
        char curr, next = default;
        Span<char> buffer = stackalloc char[str.Length * 2];

        for (idx = 0; idx < str.Length; idx++)
        {
            curr = str[idx];
            next = idx + 1 < str.Length ? str[idx + 1] : default;
            if (!Skip()) buffer[length++] = char.ToLower(curr);
            if (Kebab()) buffer[length++] = '-';
        }

        return buffer[..length].ToString();

        bool Skip () => !char.IsLetterOrDigit(curr);
        bool Kebab () => length > 0 && (
            char.IsLower(curr) && char.IsUpper(next) ||
            char.IsLetter(curr) && char.IsDigit(next) ||
            char.IsDigit(curr) && char.IsLetter(next) ||
            !char.IsLetterOrDigit(curr) && char.IsLetterOrDigit(next));
    }
}
