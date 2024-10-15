namespace Naninovel.Metadata;

/// <summary>
/// Resolves values of expressions specified in metadata models using well-known
/// constants, project metadata and parameter values of the scenario script commands.
/// </summary>
/// <param name="meta">Project metadata.</param>
/// <param name="getInspectedScript">Function to resolve path of the currently inspected scenario script.</param>
/// <param name="getParamValue">Function to resolve command parameter values.</param>
public class ExpressionEvaluator (IMetadata meta, Func<string> getInspectedScript, ExpressionEvaluator.GetParamValue getParamValue)
{
    /// <param name="id">Identifier of the resolved parameter.</param>
    /// <param name="index">When named or collection — index of the resolved value in the parameter.</param>
    /// <returns>Resolved parameter value or null when not found or the parameter is un-assigned.</returns>
    public delegate string? GetParamValue (string id, int? index = null);

    /// <summary>
    /// Expression constant resolved to the path of the currently inspected (eg, in IDE) scenario script.
    /// </summary>
    public const string InspectedScript = "$InspectedScript";
    /// <summary>
    /// Expression constant resolved to the path of the entry (start game) script.
    /// </summary>
    public const string EntryScript = "$EntryScript";
    /// <summary>
    /// Expression constant resolved to the path of the title script.
    /// </summary>
    public const string TitleScript = "$TitleScript";

    private const char expressionStartSymbol = '{';
    private const char expressionEndSymbol = '}';
    private const string concatSymbol = "+";
    private const string nullCoalescingSymbol = "??";
    private const char paramIdSymbol = ':';
    private const char paramIndexStartSymbol = '[';
    private const char paramIndexEndSymbol = ']';
    private static readonly string[] concatSeparator = [concatSymbol];
    private static readonly string[] nullSeparator = [nullCoalescingSymbol];

    /// <summary>
    /// Resolves value of the specified expression.
    /// </summary>
    /// <param name="expression">The expression to resolve.</param>
    /// <returns>The evaluated result or null.</returns>
    public string? Evaluate (string expression)
    {
        if (string.IsNullOrEmpty(expression)) return null;
        return EvaluatePart(expression);
    }

    /// <summary>
    /// Resolves values of the specified expression; supports multiple concatenated values.
    /// </summary>
    /// <param name="expression">The expression to resolve.</param>
    /// <param name="results">The collection to append resolved results.</param>
    public void Evaluate (string expression, IList<string> results)
    {
        if (string.IsNullOrEmpty(expression)) return;
        var parts = expression.Split(concatSeparator, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < parts.Length; i++)
            if (EvaluatePart(parts[i]) is { } result)
                results.Add(result);
    }

    private string? EvaluatePart (string part)
    {
        var startIndex = part.IndexOf(expressionStartSymbol);
        var endIndex = part.IndexOf(expressionEndSymbol);

        while (endIndex - startIndex > 1 && startIndex >= 0)
        {
            var expression = part.Substring(startIndex + 1, endIndex - startIndex - 1);
            part = part.Remove(startIndex, endIndex - startIndex + 1);
            part = part.Insert(startIndex, EvaluateExpression(expression));
            startIndex = part.IndexOf(expressionStartSymbol);
            endIndex = part.IndexOf(expressionEndSymbol);
        }

        return string.IsNullOrEmpty(part) ? null : part;
    }

    private string EvaluateExpression (string expression)
    {
        var atoms = expression.Split(nullSeparator, StringSplitOptions.RemoveEmptyEntries);
        foreach (var atom in atoms)
            if (EvaluateAtom(atom) is { } value)
                return value;
        return "";
    }

    private string? EvaluateAtom (string atom)
    {
        if (atom == InspectedScript) return getInspectedScript();
        if (atom == EntryScript) return meta.EntryScript;
        if (atom == TitleScript) return meta.TitleScript;
        if (!atom.StartsWith(paramIdSymbol)) return null;
        var indexStart = atom.IndexOf(paramIndexStartSymbol);
        var indexEnd = atom.IndexOf(paramIndexEndSymbol);
        var hasIndex = indexEnd - indexStart > 1 && indexStart >= 2;
        var id = hasIndex ? atom.Substring(1, indexStart - 1) : atom[1..];
        var indexString = hasIndex ? atom.Substring(indexStart + 1, indexEnd - indexStart - 1) : null;
        if (hasIndex && int.TryParse(indexString, out var index)) return getParamValue(id, index);
        return getParamValue(id);
    }
}
