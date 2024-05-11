using Naninovel.Parsing;

namespace Naninovel.Metadata;

/// <summary>
/// Hosts and provides access to current project NaniScript syntax.
/// </summary>
public class SyntaxProvider : ISyntax
{
    public string CommentLine { get; private set; } = Syntax.Default.CommentLine;
    public string LabelLine { get; private set; } = Syntax.Default.LabelLine;
    public string CommandLine { get; private set; } = Syntax.Default.CommandLine;
    public string AuthorAssign { get; private set; } = Syntax.Default.AuthorAssign;
    public string AuthorAppearance { get; private set; } = Syntax.Default.AuthorAppearance;
    public string ExpressionOpen { get; private set; } = Syntax.Default.ExpressionOpen;
    public string ExpressionClose { get; private set; } = Syntax.Default.ExpressionClose;
    public string InlinedOpen { get; private set; } = Syntax.Default.InlinedOpen;
    public string InlinedClose { get; private set; } = Syntax.Default.InlinedClose;
    public string ParameterAssign { get; private set; } = Syntax.Default.ParameterAssign;
    public string ListDelimiter { get; private set; } = Syntax.Default.ListDelimiter;
    public string NamedDelimiter { get; private set; } = Syntax.Default.NamedDelimiter;
    public string TextIdOpen { get; private set; } = Syntax.Default.TextIdOpen;
    public string TextIdClose { get; private set; } = Syntax.Default.TextIdClose;
    public string BooleanFlag { get; private set; } = Syntax.Default.BooleanFlag;
    public string True { get; private set; } = Syntax.Default.True;
    public string False { get; private set; } = Syntax.Default.False;

    public void Update (Syntax stx)
    {
        CommentLine = stx.CommentLine;
        LabelLine = stx.LabelLine;
        CommandLine = stx.CommandLine;
        AuthorAssign = stx.AuthorAssign;
        AuthorAppearance = stx.AuthorAppearance;
        ExpressionOpen = stx.ExpressionOpen;
        ExpressionClose = stx.ExpressionClose;
        InlinedOpen = stx.InlinedOpen;
        InlinedClose = stx.InlinedClose;
        ParameterAssign = stx.ParameterAssign;
        ListDelimiter = stx.ListDelimiter;
        NamedDelimiter = stx.NamedDelimiter;
        TextIdOpen = stx.TextIdOpen;
        TextIdClose = stx.TextIdClose;
        BooleanFlag = stx.BooleanFlag;
        True = stx.True;
        False = stx.False;
    }
}
