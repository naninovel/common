using System.Collections.Generic;

namespace Naninovel.Parsing.Test;

public static class LexerTestData
{
    public static IEnumerable<object[]> EmptyLines { get; } = new[] {
        T(string.Empty),
        T("    "),
        T("\t\t"),
        T(" \t  \t ")
    };

    public static IEnumerable<object[]> CommentLines { get; } = new[] {
        T(
            "; Comment",
            LineId(0, 1), CommentText(2, 7)
        ),
        T(
            "; Comment Text ",
            LineId(0, 1), CommentText(2, 12)
        ),
        T(
            "\t \t; \tComment \t Text \t ",
            LineId(3, 1), CommentText(6, 14)
        ),
        T(
            ";C",
            LineId(0, 1), CommentText(1, 1)
        ),
        T(
            ";",
            LineId(0, 1)
        ),
        T(
            "\t; \t ",
            LineId(1, 1)
        )
    };

    public static IEnumerable<object[]> LabelLines { get; } = new[] {
        T(
            "# Label",
            LineId(0, 1), LabelText(2, 5)
        ),
        T(
            "# Label Text ",
            LineId(0, 1), LabelText(2, 5), SpaceInLabel(7, 5)
        ),
        T(
            "\t \t# \tLabel \t ",
            LineId(3, 1), LabelText(6, 5)
        ),
        T(
            "#L",
            LineId(0, 1), LabelText(1, 1)
        ),
        T(
            "#",
            LineId(0, 1), MissingLabel(0, 1)
        ),
        T(
            "\t# \t ",
            LineId(1, 1), MissingLabel(0, 5)
        )
    };

    public static IEnumerable<object[]> CommandLines { get; } = new[] {
        T(
            "@command",
            LineId(0, 1), CommandId(1, 7), CommandBody(1, 7)
        ),
        T(
            "@ command",
            LineId(0, 1), CommandId(2, 7), CommandBody(1, 8)
        ),
        T(
            "@",
            LineId(0, 1), MissingCommandId(0, 1)
        ),
        T(
            "\t@ \t ",
            LineId(1, 1), MissingCommandId(1, 4), CommandBody(2, 3)
        ),
        T(
            "\t@c\tNameless.Parameter\t",
            LineId(1, 1), CommandId(2, 1),
            ParamValue(4, 18), NamelessParam(4, 18), CommandBody(2, 21)
        ),
        T(
            "@\tcommand param",
            LineId(0, 1), CommandId(2, 7),
            ParamValue(10, 5), NamelessParam(10, 5), CommandBody(1, 14)
        ),
        T(
            "@c p:\"  ",
            LineId(0, 1), CommandId(1, 1),
            ParamId(3, 1), ParamAssign(4, 1), ParamValue(5, 3), NamedParam(3, 5),
            CommandBody(1, 7)
        ),
        T(
            "@c \"na\\{less \\}ram\" ",
            LineId(0, 1), CommandId(1, 1),
            ParamValue(3, 16), NamelessParam(3, 16), CommandBody(1, 19)
        ),
        T(
            "@command param1:value\tparam2:\"v: :v\"",
            LineId(0, 1), CommandId(1, 7),
            ParamId(9, 6), ParamAssign(15, 1), ParamValue(16, 5), NamedParam(9, 12),
            ParamId(22, 6), ParamAssign(28, 1), ParamValue(29, 7), NamedParam(22, 14),
            CommandBody(1, 35)
        ),
        T(
            "@c p p:v p2:\":\"\t",
            LineId(0, 1), CommandId(1, 1),
            ParamValue(3, 1), NamelessParam(3, 1),
            ParamId(5, 1), ParamAssign(6, 1), ParamValue(7, 1), NamedParam(5, 3),
            ParamId(9, 2), ParamAssign(11, 1), ParamValue(12, 3), NamedParam(9, 6),
            CommandBody(1, 15)
        ),
        T(
            "@c \"\\\"t\\\"\"",
            LineId(0, 1), CommandId(1, 1),
            ParamValue(3, 7), NamelessParam(3, 7), CommandBody(1, 9)
        ),
        T(
            "@c p:\"     \" ",
            LineId(0, 1), CommandId(1, 1),
            ParamId(3, 1), ParamAssign(4, 1), ParamValue(5, 7), NamedParam(3, 9),
            CommandBody(1, 12)
        ),
        T(
            "@c p:{ d ? v : n }",
            LineId(0, 1), CommandId(1, 1),
            ParamId(3, 1), ParamAssign(4, 1),
            ExpressionOpen(5, 1), ExpressionBody(6, 11),
            ExpressionClose(17, 1), Expression(5, 13),
            ParamValue(5, 13), NamedParam(3, 15), CommandBody(1, 17)
        ),
        T(
            "@c v{ d ? v : n }v p:\" { a(1,2) ? \\\"test \\\" : b }\"",
            LineId(0, 1), CommandId(1, 1),
            ExpressionOpen(4, 1), ExpressionBody(5, 11),
            ExpressionClose(16, 1), Expression(4, 13),
            ParamValue(3, 15), NamelessParam(3, 15),
            ParamId(19, 1), ParamAssign(20, 1),
            ExpressionOpen(23, 1), ExpressionBody(24, 24),
            ExpressionClose(48, 1), Expression(23, 26),
            ParamValue(21, 29), NamedParam(19, 31), CommandBody(1, 49)
        ),
        T(
            "@c p:{ d ? v : n ",
            LineId(0, 1), CommandId(1, 1),
            ParamId(3, 1), ParamAssign(4, 1),
            ExpressionOpen(5, 1), ExpressionBody(6, 11), Expression(5, 12),
            CommandBody(1, 16)
        ),
        T(
            "@c p:\"{\"\\\"}\"",
            LineId(0, 1), CommandId(1, 1),
            ParamId(3, 1), ParamAssign(4, 1),
            ExpressionOpen(6, 1), ExpressionBody(7, 3),
            ExpressionClose(10, 1), Expression(6, 5),
            ParamValue(5, 7), NamedParam(3, 9), CommandBody(1, 11)
        ),
        T(
            "@c p:v p",
            LineId(0, 1), CommandId(1, 1),
            ParamId(3, 1), ParamAssign(4, 1), ParamValue(5, 1), NamedParam(3, 3),
            ParamValue(7, 1), NamelessParam(7, 1), CommandBody(1, 7)
        ),
        T(
            "@c \t:v p",
            LineId(0, 1), CommandId(1, 1),
            MissingParamId(4, 1), ParamAssign(4, 1), ParamValue(5, 1), NamedParam(4, 2),
            ParamValue(7, 1), NamelessParam(7, 1), CommandBody(1, 7)
        ),
        T(
            "@c p p:v \" \"",
            LineId(0, 1), CommandId(1, 1),
            ParamValue(3, 1), NamelessParam(3, 1),
            ParamId(5, 1), ParamAssign(6, 1), ParamValue(7, 1), NamedParam(5, 3),
            MultipleNameless(9, 3), CommandBody(1, 11)
        ),
        T(
            "@c a=\" \";b=\" \"",
            LineId(0, 1), CommandId(1, 1),
            ParamValue(3, 11), NamelessParam(3, 11),
            CommandBody(1, 13)
        ),
        T(
            "@c p:",
            LineId(0, 1), CommandId(1, 1),
            ParamId(3, 1), ParamAssign(4, 1), MissingParamValue(3, 2),
            NamedParam(3, 2), CommandBody(1, 4)
        )
    };

    public static IEnumerable<object[]> GenericTextLines { get; } = new[] {
        T(
            "Generic text line.",
            GenericText(0, 18)
        ),
        T(
            " g\t ",
            GenericText(1, 1)
        ),
        T(
            "\\; x",
            GenericText(0, 4)
        ),
        T(
            "\\# x",
            GenericText(0, 4)
        ),
        T(
            "\\@ x",
            GenericText(0, 4)
        ),
        T(
            "A{ a ? b() : c } ",
            ExpressionOpen(1, 1), ExpressionBody(2, 13),
            ExpressionClose(15, 1), Expression(1, 15), GenericText(0, 16)
        ),
        T(
            "A{A}",
            ExpressionOpen(1, 1), ExpressionBody(2, 1),
            ExpressionClose(3, 1), Expression(1, 3), GenericText(0, 4)
        ),
        T(
            "{}",
            ExpressionOpen(0, 1), ExpressionClose(1, 1), Expression(0, 2),
            MissingExpressionBody(0, 2), GenericText(0, 2)
        ),
        T(
            "{ \t}",
            ExpressionOpen(0, 1), ExpressionClose(3, 1), Expression(0, 4),
            MissingExpressionBody(0, 4), GenericText(0, 4)
        ),
        T(
            "{\\}}",
            ExpressionOpen(0, 1), ExpressionBody(1, 2), ExpressionClose(3, 1),
            Expression(0, 4), GenericText(0, 4)
        ),
        T(
            "\\{{a}\\}b",
            ExpressionOpen(2, 1), ExpressionBody(3, 1), ExpressionClose(4, 1),
            Expression(2, 3), GenericText(0, 8)
        ),
        T(
            "Text[i]",
            GenericText(0, 4), InlinedOpen(4, 1), CommandId(5, 1),
            CommandBody(5, 1), InlinedClose(6, 1), Inlined(4, 3)
        ),
        T(
            "[i]",
            InlinedOpen(0, 1), CommandId(1, 1),
            CommandBody(1, 1), InlinedClose(2, 1), Inlined(0, 3)
        ),
        T(
            "[i][ i ]",
            InlinedOpen(0, 1), CommandId(1, 1),
            CommandBody(1, 1), InlinedClose(2, 1), Inlined(0, 3),
            InlinedOpen(3, 1), CommandId(5, 1),
            CommandBody(4, 3), InlinedClose(7, 1), Inlined(3, 5)
        ),
        T(
            "x[x]x [x]x",
            GenericText(0, 1), InlinedOpen(1, 1), CommandId(2, 1),
            CommandBody(2, 1), InlinedClose(3, 1), Inlined(1, 3),
            GenericText(4, 2), InlinedOpen(6, 1), CommandId(7, 1),
            CommandBody(7, 1), InlinedClose(8, 1), Inlined(6, 3), GenericText(9, 1)
        ),
        T(
            "[\tx \" \" x:x{x} ]\\[[x\t]\\]",
            InlinedOpen(0, 1), CommandId(2, 1),
            ParamValue(4, 3), NamelessParam(4, 3),
            ParamId(8, 1), ParamAssign(9, 1),
            ExpressionOpen(11, 1), ExpressionBody(12, 1),
            ExpressionClose(13, 1), Expression(11, 3),
            ParamValue(10, 4), NamedParam(8, 6),
            CommandBody(1, 14), InlinedClose(15, 1), Inlined(0, 16),
            GenericText(16, 2),
            InlinedOpen(18, 1), CommandId(19, 1),
            CommandBody(19, 2), InlinedClose(21, 1), Inlined(18, 4),
            GenericText(22, 2)
        ),
        T(
            "[]",
            InlinedOpen(0, 1), MissingCommandId(0, 1),
            InlinedClose(1, 1), Inlined(0, 2)
        ),
        T(
            "[c x",
            InlinedOpen(0, 1), CommandId(1, 1),
            ParamValue(3, 1), NamelessParam(3, 1),
            CommandBody(1, 3), Inlined(0, 4)
        ),
        T(
            "[c p:]",
            InlinedOpen(0, 1), CommandId(1, 1),
            ParamId(3, 1), ParamAssign(4, 1), MissingParamValue(3, 2), NamedParam(3, 2),
            CommandBody(1, 4), InlinedClose(5, 1), Inlined(0, 6)
        ),
        T(
            "[\t ]",
            InlinedOpen(0, 1), MissingCommandId(0, 3), CommandBody(1, 2),
            InlinedClose(3, 1), Inlined(0, 4)
        ),
        T(
            "x: x",
            AuthorId(0, 1), AuthorAssign(1, 2), GenericText(3, 1)
        ),
        T(
            "x.x: x",
            AuthorId(0, 1), AppearanceAssign(1, 1), AuthorAppearance(2, 1),
            AuthorAssign(3, 2), GenericText(5, 1)
        ),
        T(
            "x.x/x: x",
            AuthorId(0, 1), AppearanceAssign(1, 1), AuthorAppearance(2, 3),
            AuthorAssign(5, 2), GenericText(7, 1)
        ),
        T(
            "x.x+x,x-,x>: x",
            AuthorId(0, 1), AppearanceAssign(1, 1), AuthorAppearance(2, 9),
            AuthorAssign(11, 2), GenericText(13, 1)
        ),
        T(
            "x.: x",
            AuthorId(0, 1), AppearanceAssign(1, 1), MissingAppearance(1, 1),
            AuthorAssign(2, 2), GenericText(4, 1)
        ),
        T(
            ": x",
            GenericText(0, 3)
        ),
        T(
            "x:x",
            GenericText(0, 3)
        ),
        T(
            "\"x: x",
            GenericText(0, 5)
        ),
        T(
            "x\\: x",
            GenericText(0, 5)
        ),
        T(
            "x.\": x",
            GenericText(0, 6)
        )
    };

    private static object[] T (string text, params Token[] tokens)
    {
        var testData = new object[tokens.Length + 1];
        testData[0] = text;
        for (int i = 0; i < tokens.Length; i++)
            testData[i + 1] = tokens[i];
        return testData;
    }

    private static Token Token (TokenType type, int startIndex, int length) => new(type, startIndex, length);
    private static Token LabelText (int startIndex, int length) => Token(TokenType.LabelText, startIndex, length);
    private static Token CommentText (int startIndex, int length) => Token(TokenType.CommentText, startIndex, length);
    private static Token LineId (int startIndex, int length) => Token(TokenType.LineId, startIndex, length);
    private static Token ExpressionBody (int startIndex, int length) => Token(TokenType.ExpressionBody, startIndex, length);
    private static Token Expression (int startIndex, int length) => Token(TokenType.Expression, startIndex, length);
    private static Token ExpressionOpen (int startIndex, int length) => Token(TokenType.ExpressionOpen, startIndex, length);
    private static Token ExpressionClose (int startIndex, int length) => Token(TokenType.ExpressionClose, startIndex, length);
    private static Token ParamValue (int startIndex, int length) => Token(TokenType.ParamValue, startIndex, length);
    private static Token NamelessParam (int startIndex, int length) => Token(TokenType.NamelessParam, startIndex, length);
    private static Token NamedParam (int startIndex, int length) => Token(TokenType.NamedParam, startIndex, length);
    private static Token ParamAssign (int startIndex, int length) => Token(TokenType.ParamAssign, startIndex, length);
    private static Token ParamId (int startIndex, int length) => Token(TokenType.ParamId, startIndex, length);
    private static Token CommandId (int startIndex, int length) => Token(TokenType.CommandId, startIndex, length);
    private static Token CommandBody (int startIndex, int length) => Token(TokenType.CommandBody, startIndex, length);
    private static Token Inlined (int startIndex, int length) => Token(TokenType.Inlined, startIndex, length);
    private static Token InlinedOpen (int startIndex, int length) => Token(TokenType.InlinedOpen, startIndex, length);
    private static Token InlinedClose (int startIndex, int length) => Token(TokenType.InlinedClose, startIndex, length);
    private static Token AuthorAssign (int startIndex, int length) => Token(TokenType.AuthorAssign, startIndex, length);
    private static Token AuthorAppearance (int startIndex, int length) => Token(TokenType.AuthorAppearance, startIndex, length);
    private static Token AppearanceAssign (int startIndex, int length) => Token(TokenType.AppearanceAssign, startIndex, length);
    private static Token AuthorId (int startIndex, int length) => Token(TokenType.AuthorId, startIndex, length);
    private static Token GenericText (int startIndex, int length) => Token(TokenType.GenericText, startIndex, length);

    private static Token Error (ErrorType type, int startIndex, int length) => new(type, startIndex, length);
    private static Token MissingParamId (int startIndex, int length) => Error(ErrorType.MissingParamId, startIndex, length);
    private static Token SpaceInLabel (int startIndex, int length) => Error(ErrorType.SpaceInLabel, startIndex, length);
    private static Token MissingLabel (int startIndex, int length) => Error(ErrorType.MissingLabel, startIndex, length);
    private static Token MultipleNameless (int startIndex, int length) => Error(ErrorType.MultipleNameless, startIndex, length);
    private static Token MissingExpressionBody (int startIndex, int length) => Error(ErrorType.MissingExpressionBody, startIndex, length);
    private static Token MissingParamValue (int startIndex, int length) => Error(ErrorType.MissingParamValue, startIndex, length);
    private static Token MissingCommandId (int startIndex, int length) => Error(ErrorType.MissingCommandId, startIndex, length);
    private static Token MissingAppearance (int startIndex, int length) => Error(ErrorType.MissingAppearance, startIndex, length);
}
