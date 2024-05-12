namespace Naninovel.Parsing;

/// <inheritdoc cref="ISyntax"/>
public class Syntax : ISyntax
{
    /// <summary>
    /// Canonical syntax.
    /// </summary>
    public static readonly Syntax Default = new();

    public string CommentLine { get; } = ";";
    public string LabelLine { get; } = "#";
    public string CommandLine { get; } = "@";
    public string AuthorAssign { get; } = ": ";
    public string AuthorAppearance { get; } = ".";
    public string ParametrizeGeneric { get; } = "<";
    public string ExpressionOpen { get; } = "{";
    public string ExpressionClose { get; } = "}";
    public string InlinedOpen { get; } = "[";
    public string InlinedClose { get; } = "]";
    public string ParameterAssign { get; } = ":";
    public string ListDelimiter { get; } = ",";
    public string NamedDelimiter { get; } = ".";
    public string TextIdOpen { get; } = "|#";
    public string TextIdClose { get; } = "|";
    public string BooleanFlag { get; } = "!";
    public string True { get; } = "true";
    public string False { get; } = "false";

    // TODO: Change to init-only props on runtime upgrade.
    public Syntax (
        string? commentLine = null,
        string? labelLine = null,
        string? commandLine = null,
        string? authorAssign = null,
        string? authorAppearance = null,
        string? parametrizeGeneric = null,
        string? expressionOpen = null,
        string? expressionClose = null,
        string? inlinedOpen = null,
        string? inlinedClose = null,
        string? parameterAssign = null,
        string? listDelimiter = null,
        string? namedDelimiter = null,
        string? textIdOpen = null,
        string? textIdClose = null,
        string? booleanFlag = null,
        string? @true = null,
        string? @false = null)
    {
        CommentLine = commentLine ?? CommentLine;
        LabelLine = labelLine ?? LabelLine;
        CommandLine = commandLine ?? CommandLine;
        AuthorAssign = authorAssign ?? AuthorAssign;
        AuthorAppearance = authorAppearance ?? AuthorAppearance;
        ParametrizeGeneric = parametrizeGeneric ?? ParametrizeGeneric;
        ExpressionOpen = expressionOpen ?? ExpressionOpen;
        ExpressionClose = expressionClose ?? ExpressionClose;
        InlinedOpen = inlinedOpen ?? InlinedOpen;
        InlinedClose = inlinedClose ?? InlinedClose;
        ParameterAssign = parameterAssign ?? ParameterAssign;
        ListDelimiter = listDelimiter ?? ListDelimiter;
        NamedDelimiter = namedDelimiter ?? NamedDelimiter;
        TextIdOpen = textIdOpen ?? TextIdOpen;
        TextIdClose = textIdClose ?? TextIdClose;
        BooleanFlag = booleanFlag ?? BooleanFlag;
        True = @true ?? True;
        False = @false ?? False;
    }
}
