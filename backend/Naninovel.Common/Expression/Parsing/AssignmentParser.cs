namespace Naninovel.Expression;

internal class AssignmentParser
{
    private static readonly char[] separator = [';'];

    public void Parse (string text, IList<(string var, string text)> asses)
    {
        var texts = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        foreach (var t in texts)
            asses.Add(BuildAssignment(t));
    }

    private (string var, string text) BuildAssignment (string text)
    {
        ProcessUnaryOperators(ref text);
        ExtractVariableAndExpression(text, out var var, out var exp);
        ProcessCompoundAssignment(ref var, ref exp);
        return (var, exp);
    }

    private void ProcessUnaryOperators (ref string text)
    {
        if (text.EndsWith("++", StringComparison.Ordinal))
            text = text.Replace("++", $"={text.GetBefore("++")}+1");
        else if (text.EndsWith("--", StringComparison.Ordinal))
            text = text.Replace("--", $"={text.GetBefore("--")}-1");
    }

    private void ExtractVariableAndExpression (string text, out string var, out string exp)
    {
        var = text.GetBefore("=").Trim();
        if (string.IsNullOrWhiteSpace(var))
            throw new Error("Missing assigned variable name.");
        exp = text.GetAfterFirst("=").Trim();
        if (string.IsNullOrWhiteSpace(exp))
            throw new Error("Missing expression to assign.");
    }

    private void ProcessCompoundAssignment (ref string var, ref string exp)
    {
        if (!var.EndsWith("+", StringComparison.Ordinal) && !var.EndsWith("-", StringComparison.Ordinal) &&
            !var.EndsWith("*", StringComparison.Ordinal) && !var.EndsWith("/", StringComparison.Ordinal)) return;
        exp = var + exp;
        var = var.Substring(0, var.Length - 1).Trim();
    }
}
