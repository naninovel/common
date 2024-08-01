namespace Naninovel.Metadata;

public static class ConstantEvaluator
{
    public delegate string? GetParamValue (string id, int? index = null);

    private const char expressionStartSymbol = '{';
    private const char expressionEndSymbol = '}';
    private const string concatSymbol = "+";
    private const string nullCoalescingSymbol = "??";
    private const string scriptSymbol = "$Script";
    private const char paramIdSymbol = ':';
    private const char paramIndexStartSymbol = '[';
    private const char paramIndexEndSymbol = ']';
    private static readonly string[] concatSeparator = [concatSymbol];
    private static readonly string[] nullSeparator = [nullCoalescingSymbol];

    public static IReadOnlyList<string> EvaluateNames (string value, string scriptId, GetParamValue getParamValue)
    {
        if (string.IsNullOrEmpty(value)) return Array.Empty<string>();
        var parts = value.Split(concatSeparator, StringSplitOptions.RemoveEmptyEntries).ToList();
        for (int i = parts.Count - 1; i >= 0; i--)
            if (EvaluatePart(parts[i], scriptId, getParamValue) is { } part) parts[i] = part;
            else parts.RemoveAt(i);
        return parts;
    }

    private static string? EvaluatePart (string part, string scriptId, GetParamValue getParamValue)
    {
        var startIndex = part.IndexOf(expressionStartSymbol);
        var endIndex = part.IndexOf(expressionEndSymbol);

        while (endIndex - startIndex > 1 && startIndex >= 0)
        {
            var expression = part.Substring(startIndex + 1, endIndex - startIndex - 1);
            part = part.Remove(startIndex, endIndex - startIndex + 1);
            part = part.Insert(startIndex, EvaluateExpression(expression, scriptId, getParamValue));
            startIndex = part.IndexOf(expressionStartSymbol);
            endIndex = part.IndexOf(expressionEndSymbol);
        }

        return string.IsNullOrEmpty(part) ? null : part;
    }

    private static string EvaluateExpression (string expression, string scriptId, GetParamValue getParamValue)
    {
        var atoms = expression.Split(nullSeparator, StringSplitOptions.RemoveEmptyEntries);
        foreach (var atom in atoms)
            if (EvaluateAtom(atom, scriptId, getParamValue) is { } value)
                return value;
        return "";
    }

    private static string? EvaluateAtom (string atom, string scriptId, GetParamValue getParamValue)
    {
        if (atom == scriptSymbol) return scriptId;
        if (!atom.StartsWith(paramIdSymbol.ToString())) return null;
        var indexStart = atom.IndexOf(paramIndexStartSymbol);
        var indexEnd = atom.IndexOf(paramIndexEndSymbol);
        var hasIndex = indexEnd - indexStart > 1 && indexStart >= 2;
        var id = hasIndex ? atom.Substring(1, indexStart - 1) : atom.Substring(1);
        var indexString = hasIndex ? atom.Substring(indexStart + 1, indexEnd - indexStart - 1) : null;
        if (hasIndex && int.TryParse(indexString, out var index)) return getParamValue(id, index);
        return getParamValue(id);
    }
}
