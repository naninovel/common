using System;

namespace Naninovel.Metadata;

public static class ConstantEvaluator
{
    public delegate string GetParamValue (string id, int? index = null);

    private const char expressionStartSymbol = '{';
    private const char expressionEndSymbol = '}';
    private const string nullCoalescingSymbol = "??";
    private const string scriptSymbol = "$Script";
    private const char paramIdSymbol = ':';
    private const char paramIndexStartSymbol = '[';
    private const char paramIndexEndSymbol = ']';

    public static string EvaluateName (string value, string script, GetParamValue getParamValue)
    {
        if (string.IsNullOrEmpty(value)) return value;

        var startIndex = value.IndexOf(expressionStartSymbol);
        var endIndex = value.IndexOf(expressionEndSymbol);

        while (endIndex - startIndex > 1 && startIndex >= 0)
        {
            var expression = value.Substring(startIndex + 1, endIndex - startIndex - 1);
            value = value.Remove(startIndex, endIndex - startIndex + 1);
            value = value.Insert(startIndex, EvaluateExpression(expression, script, getParamValue));
            startIndex = value.IndexOf(expressionStartSymbol);
            endIndex = value.IndexOf(expressionEndSymbol);
        }

        return value;
    }

    private static string EvaluateExpression (string expression, string script, GetParamValue getParamValue)
    {
        var atoms = expression.Split(new[] { nullCoalescingSymbol }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var atom in atoms)
            if (EvaluateAtom(atom, script, getParamValue) is { } value)
                return value;
        return "";
    }

    private static string EvaluateAtom (string atom, string script, GetParamValue getParamValue)
    {
        if (atom == scriptSymbol) return script;
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
