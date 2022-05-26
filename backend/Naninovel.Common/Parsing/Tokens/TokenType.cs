using System;

namespace Naninovel.Parsing;

[Flags]
public enum TokenType
{
    LineId = 1 << 0,
    CommentText = 1 << 1,
    LabelText = 1 << 2,
    GenericText = 1 << 3,
    CommandBody = 1 << 4,
    CommandId = 1 << 5,
    NamelessParam = 1 << 6,
    NamedParam = 1 << 7,
    ParamId = 1 << 8,
    ParamValue = 1 << 9,
    ParamAssign = 1 << 10,
    Expression = 1 << 11,
    ExpressionBody = 1 << 12,
    ExpressionOpen = 1 << 13,
    ExpressionClose = 1 << 14,
    Inlined = 1 << 15,
    InlinedOpen = 1 << 16,
    InlinedClose = 1 << 17,
    AuthorId = 1 << 18,
    AuthorAppearance = 1 << 19,
    AuthorAssign = 1 << 20,
    AppearanceAssign = 1 << 21,
    Error = 1 << 22
}

public static class TokenTypeExtensions
{
    public static bool HasFlag (this TokenType type, TokenType flag)
    {
        return (type & flag) != 0;
    }
}