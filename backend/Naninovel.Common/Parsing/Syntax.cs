namespace Naninovel.Parsing;

/// <inheritdoc cref="ISyntax"/>
public class Syntax : ISyntax
{
    /// <summary>
    /// Canonical syntax.
    /// </summary>
    public static readonly Syntax Default = new();

    public string CommentLine { get; set; } = ";";
    public string LabelLine { get; set; } = "#";
    public string CommandLine { get; set; } = "@";
    public string AuthorAssign { get; set; } = ": ";
    public string AuthorAppearance { get; set; } = ".";
    public string ParametrizeGeneric { get; set; } = "<";
    public string ExpressionOpen { get; set; } = "{";
    public string ExpressionClose { get; set; } = "}";
    public string InlinedOpen { get; set; } = "[";
    public string InlinedClose { get; set; } = "]";
    public string ParameterAssign { get; set; } = ":";
    public string ListDelimiter { get; set; } = ",";
    public string NamedDelimiter { get; set; } = ".";
    public string TextIdOpen { get; set; } = "|#";
    public string TextIdClose { get; set; } = "|";
    public string BooleanFlag { get; set; } = "!";
    public string True { get; set; } = "true";
    public string False { get; set; } = "false";
}
