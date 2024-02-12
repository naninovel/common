namespace Naninovel.Parsing;

[Flags]
public enum TokenType
{
    Error = 1 << 0,
    LineId = 1 << 1,
    CommentText = 1 << 2,
    LabelText = 1 << 3,
    GenericText = 1 << 4,
    CommandBody = 1 << 5,
    CommandId = 1 << 6,
    NamelessParam = 1 << 7,
    NamedParam = 1 << 8,
    ParamId = 1 << 9,
    ParamValue = 1 << 10,
    ParamAssign = 1 << 11,
    Expression = 1 << 12,
    ExpressionBody = 1 << 13,
    ExpressionOpen = 1 << 14,
    ExpressionClose = 1 << 15,
    Inlined = 1 << 16,
    InlinedOpen = 1 << 17,
    InlinedClose = 1 << 18,
    AuthorId = 1 << 19,
    AuthorAppearance = 1 << 20,
    AuthorAssign = 1 << 21,
    AppearanceAssign = 1 << 22,
    TextId = 1 << 23,
    TextIdBody = 1 << 24,
    TextIdOpen = 1 << 25,
    TextIdClose = 1 << 26,
    WaitTrue = 1 << 27,
    WaitFalse = 1 << 28
}

public static class TokenTypeExtensions
{
    public static bool HasFlag (this TokenType type, TokenType flag)
    {
        return (type & flag) != 0;
    }
}
