using Naninovel.Parsing;

namespace Naninovel.Metadata;

/// <summary>
/// Resolves values of expressions specified in metadata models using project metadata
/// and parameter values of the scenario script commands or script expression functions.
/// </summary>
/// <param name="meta">Project metadata.</param>
public class ExpressionEvaluator (IMetadata meta)
{
    /// <summary>
    /// The context in which expression is evaluated.
    /// </summary>
    public struct Context
    {
        /// <summary>
        /// Specify in case the evaluated expression is expected to access scenario script
        /// command parameter values; not compatible with <see cref="Function"/>.
        /// </summary>
        public Parsing.Command? Command { get; set; }
        /// <summary>
        /// Specify in case the evaluated expression is expected to access script expression
        /// function parameter values; not compatible with <see cref="Command"/>.
        /// </summary>
        public Expression.Function? Function { get; set; }
    }

    /// <summary>
    /// Expression constant resolved to <see cref="IMetadata.EntryScript"/>.
    /// </summary>
    public const string EntryScript = "$EntryScript";
    /// <summary>
    /// Expression constant resolved to <see cref="IMetadata.TitleScript"/>.
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

    private readonly NamedValueParser namedParser = new(meta.Syntax);
    private Context ctx;

    /// <summary>
    /// Resolves value of the specified expression.
    /// </summary>
    /// <param name="expression">The expression to resolve.</param>
    /// <param name="context">The evaluation context.</param>
    /// <returns>The evaluated result or null when failed.</returns>
    public string? Evaluate (string expression, Context context = default)
    {
        if (string.IsNullOrEmpty(expression)) return null;
        ctx = context;
        return EvaluatePart(expression);
    }

    /// <summary>
    /// Resolves values of the specified expression; supports multiple concatenated parts.
    /// </summary>
    /// <param name="expression">The expression to resolve.</param>
    /// <param name="context">The evaluation context.</param>
    /// <param name="results">The collection to append resolved results.</param>
    public void Evaluate (string expression, IList<string> results, Context context = default)
    {
        if (string.IsNullOrEmpty(expression)) return;
        ctx = context;
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
        if (atom == EntryScript) return meta.EntryScript;
        if (atom == TitleScript) return meta.TitleScript;
        if (!atom.StartsWith(paramIdSymbol)) return null;
        var indexStart = atom.IndexOf(paramIndexStartSymbol);
        var indexEnd = atom.IndexOf(paramIndexEndSymbol);
        var hasIndex = indexEnd - indexStart > 1 && indexStart >= 2;
        var id = hasIndex ? atom.Substring(1, indexStart - 1) : atom[1..];
        var indexString = hasIndex ? atom.Substring(indexStart + 1, indexEnd - indexStart - 1) : null;
        if (hasIndex && int.TryParse(indexString, out var idx)) return GetParamValue(id, idx);
        return GetParamValue(id, null);
    }

    private string? GetParamValue (string id, int? idx)
    {
        if (ctx.Command is { } cmd) return GetParamValue(cmd, id, idx);
        if (ctx.Function is { } fn) return GetParamValue(fn, id, idx);
        return null;
    }

    private string? GetParamValue (Parsing.Command cmd, string id, int? idx)
    {
        if (meta.FindCommand(cmd.Identifier) is not { } cmdMeta) return null;
        foreach (var param in cmd.Parameters)
            if (meta.FindParameter(cmdMeta.Id, param.Identifier) is { } paramMeta && paramMeta.Id == id)
                return idx.HasValue ? GetNamedValue(string.Concat(param.Value), idx.Value) : string.Concat(param.Value);
        return null;
    }

    private string? GetParamValue (Expression.Function fn, string id, int? idx)
    {
        using var _ = ListPool<Function>.Rent(out var fnMetas);
        if (!meta.FindFunctions(fn.Name, fnMetas)) return null;
        var fnMeta = fnMetas[0]; // We don't care about overloads, as we only need param names, which are equal.
        var paramIdx = -1; // Resolve param index based on the ID, which is the C# arg name of the associated expression method.
        for (int i = 0; i < fnMeta.Parameters.Length; i++)
        {
            if (!string.Equals(fnMeta.Parameters[i].Name, id, StringComparison.OrdinalIgnoreCase)) continue;
            paramIdx = i;
            break;
        }
        if (fn.Parameters.ElementAtOrDefault(paramIdx) is not Expression.String { Value: { } value }) return null;
        return idx.HasValue ? GetNamedValue(value, idx.Value) : value;
    }

    private string? GetNamedValue (string raw, int idx)
    {
        if (idx > 1) return null;
        var (name, value) = namedParser.Parse(raw);
        return idx > 0 ? value : name;
    }
}
