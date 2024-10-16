using static Naninovel.Parsing.TokenType;
using static Naninovel.Parsing.ErrorType;
using static Naninovel.Parsing.ParsingErrors;

namespace Naninovel.Parsing.Test;

public class CommandLineParserTest
{
    private readonly ParseTestHelper<CommandLine> parser = new(h => new CommandLineParser(h).Parse);

    [Fact]
    public void WhenLineIdIsMissingErrorIsAddedAndCommandIsEmpty ()
    {
        var line = parser.Parse("foo");
        Assert.Empty(line.Command.Identifier);
        Assert.Empty(line.Command.Parameters);
        Assert.True(parser.HasError(MissingLineId));
    }

    [Fact]
    public void WhenMissingCommandIdErrorIsAddedAndCommandIsEmpty ()
    {
        var line = parser.Parse("@ ");
        Assert.Empty(line.Command.Identifier);
        Assert.Empty(line.Command.Parameters);
        Assert.True(parser.HasError(MissingCommandId));
    }

    [Fact]
    public void WhenCommandTokensMissingErrorIsAdded ()
    {
        var errors = new ErrorCollector();
        var parser = new CommandLineParser(new() { Handlers = new() { ErrorHandler = errors } });
        parser.Parse("@", [new Token(LineId, 0, 1)]);
        Assert.Contains(MissingCommandTokens, errors.Select(e => e.Message));
    }

    [Fact]
    public void CanParseCommandIdentifier ()
    {
        var line = parser.Parse("@foo");
        Assert.Equal("foo", line.Command.Identifier);
        Assert.Empty(line.Command.Parameters);
        Assert.Empty(parser.Errors);
    }

    [Fact]
    public void TrimsSpaceBeforeCommandIdentifier ()
    {
        var line = parser.Parse("@ \t foo");
        Assert.Equal("foo", line.Command.Identifier);
        Assert.Empty(line.Command.Parameters);
        Assert.Empty(parser.Errors);
    }

    [Fact]
    public void WhenMissingParameterIdErrorIsAdded ()
    {
        parser.Parse("@c :v");
        Assert.True(parser.HasError(MissingParamId));
    }

    [Fact]
    public void WhenMissingParameterValueErrorIsAddedButIdIsParsed ()
    {
        var line = parser.Parse("@c p:");
        Assert.True(parser.HasError(MissingParamValue));
        Assert.Equal("p", line.Command.Parameters[0].Identifier);
    }

    [Fact]
    public void WhenMissingExpressionBodyErrorIsAdded ()
    {
        parser.Parse("@c {}");
        Assert.True(parser.HasError(MissingExpressionBody));
    }

    [Fact]
    public void WhenMultipleNamelessErrorIsAdded ()
    {
        parser.Parse("@c x y:z w");
        Assert.True(parser.HasError(MultipleNameless));
    }

    [Fact]
    public void WhenMissingTextIdBodyErrorIsAdded ()
    {
        parser.Parse("@c |#|");
        Assert.True(parser.HasError(MissingTextIdBody));
    }

    [Fact]
    public void NamelessParameterDetected ()
    {
        var line = parser.Parse("@c x p:x");
        Assert.True(line.Command.Parameters[0].Nameless);
        Assert.False(line.Command.Parameters[1].Nameless);
    }

    [Fact]
    public void DynamicParameterDetected ()
    {
        var line = parser.Parse("@c x p:x{x}x");
        Assert.False(line.Command.Parameters[0].Value.Dynamic);
        Assert.True(line.Command.Parameters[1].Value.Dynamic);
    }

    [Fact]
    public void IdentifiedTextParsed ()
    {
        var line = parser.Parse("@c 1|#id1|2|#id2| p:3|#id3|");
        Assert.Equal("1", ((IdentifiedText)line.Command.Parameters[0].Value[0]).Text);
        Assert.Equal("id1", ((IdentifiedText)line.Command.Parameters[0].Value[0]).Id.Body);
        Assert.Equal("2", ((IdentifiedText)line.Command.Parameters[0].Value[1]).Text);
        Assert.Equal("id2", ((IdentifiedText)line.Command.Parameters[0].Value[1]).Id.Body);
        Assert.Equal("3", ((IdentifiedText)line.Command.Parameters[1].Value[0]).Text);
        Assert.Equal("id3", ((IdentifiedText)line.Command.Parameters[1].Value[0]).Id.Body);
    }

    [Fact]
    public void UnclosedIdentifiedTextParsed ()
    {
        var line = parser.Parse("@c 1|#id1");
        Assert.Equal("1", ((IdentifiedText)line.Command.Parameters[0].Value[0]).Text);
        Assert.Equal("id1", ((IdentifiedText)line.Command.Parameters[0].Value[0]).Id.Body);
    }

    [Fact]
    public void EmptyIdentifiedTextParsed ()
    {
        var line = parser.Parse("@c |#|");
        Assert.Empty(((IdentifiedText)line.Command.Parameters[0].Value[0]).Text);
        Assert.Empty(((IdentifiedText)line.Command.Parameters[0].Value[0]).Id.Body);
    }

    [Fact]
    public void UnclosedEmptyIdentifiedTextParsed ()
    {
        var line = parser.Parse("@c |#");
        Assert.Empty(((IdentifiedText)line.Command.Parameters[0].Value[0]).Text);
        Assert.Empty(((IdentifiedText)line.Command.Parameters[0].Value[0]).Id.Body);
    }

    [Fact]
    public void EmptyAndUnclosedIdentifiedTextsParsed ()
    {
        var line = parser.Parse("@c 1|#|2|#id2");
        Assert.Equal("1", ((IdentifiedText)line.Command.Parameters[0].Value[0]).Text);
        Assert.Empty(((IdentifiedText)line.Command.Parameters[0].Value[0]).Id.Body);
        Assert.Equal("2", ((IdentifiedText)line.Command.Parameters[0].Value[1]).Text);
        Assert.Equal("id2", ((IdentifiedText)line.Command.Parameters[0].Value[1]).Id.Body);
    }

    [Fact]
    public void EscapedTextIdentifierParsed ()
    {
        var line = parser.Parse("@c 1\\|#id1|");
        Assert.Single(line.Command.Parameters[0].Value);
        Assert.Equal("1|#id1|", line.Command.Parameters[0].Value[0] as PlainText);
    }

    [Fact]
    public void PositiveBooleanFlagParsed ()
    {
        var line = parser.Parse("@c p!");
        Assert.Equal("p", line.Command.Parameters[0].Identifier);
        Assert.Equal("true", line.Command.Parameters[0].Value[0] as PlainText);
        Assert.Empty(parser.Errors);
    }

    [Fact]
    public void NegativeBooleanFlagParsed ()
    {
        var line = parser.Parse("@c !p");
        Assert.Equal("p", line.Command.Parameters[0].Identifier);
        Assert.Equal("false", line.Command.Parameters[0].Value[0] as PlainText);
        Assert.Empty(parser.Errors);
    }

    [Fact]
    public void CommandIsParsed ()
    {
        var line = parser.Parse("@char k.Happy pos:{x},10 wait:false");
        Assert.Equal("char", line.Command.Identifier);
        Assert.True(line.Command.Parameters[0].Nameless);
        Assert.False(line.Command.Parameters[0].Value.Dynamic);
        Assert.Equal("k.Happy", line.Command.Parameters[0].Value[0] as PlainText);
        Assert.Equal("pos", line.Command.Parameters[1].Identifier);
        Assert.Equal("x", (line.Command.Parameters[1].Value[0] as Expression)!.Body);
        Assert.Equal(",10", line.Command.Parameters[1].Value[1] as PlainText);
        Assert.Equal("wait", line.Command.Parameters[2].Identifier);
        Assert.Equal("false", line.Command.Parameters[2].Value[0] as PlainText);
    }

    [Fact]
    public void CommandWithExpressionsIsParsed ()
    {
        var line = parser.Parse("@c {x} x:x{y}x{z}");
        Assert.True(line.Command.Parameters[0].Value.Dynamic);
        Assert.Single(line.Command.Parameters[0].Value.OfType<Expression>());
        Assert.Equal("x", (line.Command.Parameters[0].Value[0] as Expression)!.Body);
        Assert.True(line.Command.Parameters[1].Value.Dynamic);
        Assert.Equal(2, line.Command.Parameters[1].Value.OfType<Expression>().Count());
        Assert.Equal("x", line.Command.Parameters[1].Value[0] as PlainText);
        Assert.Equal("y", (line.Command.Parameters[1].Value[1] as Expression)!.Body);
        Assert.Equal("x", line.Command.Parameters[1].Value[2] as PlainText);
        Assert.Equal("z", (line.Command.Parameters[1].Value[3] as Expression)!.Body);
    }

    [Fact]
    public void NamelessParameterAfterNamedParsed ()
    {
        var line = parser.Parse("@c x:y z");
        Assert.True(line.Command.Parameters[1].Nameless);
        Assert.Equal("z", line.Command.Parameters[1].Value[0] as PlainText);
    }

    [Fact]
    public void PlainTextIsDecoded ()
    {
        var line = parser.Parse("""@c "x \" \{ \} \\" p:"\{x\}\\""");
        Assert.Equal(@"x "" { } \", line.Command.Parameters[0].Value[0] as PlainText);
        Assert.Equal(@"""{x}\", line.Command.Parameters[1].Value[0] as PlainText);
    }

    [Fact]
    public void SingleQuoteValueIsParsed ()
    {
        var line = parser.Parse("""@c "\"" p:v""");
        Assert.Equal(@"""", line.Command.Parameters[0].Value[0] as PlainText);
    }

    [Fact]
    public void ExpressionWithQuotesIsParsed ()
    {
        var line = parser.Parse("""@c " \" { Random(var, " \" ") } \" " p:v""");
        Assert.Equal(@" "" ", line.Command.Parameters[0].Value[0] as PlainText);
        Assert.Equal(""" Random(var, " \" ") """, (line.Command.Parameters[0].Value[1] as Expression)!.Body);
        Assert.Equal(@" "" ", line.Command.Parameters[0].Value[2] as PlainText);
    }

    [Fact]
    public void RangesAreAssociated ()
    {
        var line = parser.Parse("@c v|#i| p:v{x} p! !p");
        Assert.Equal(new(1, 20), parser.Resolve(line.Command));
        Assert.Equal(new(1, 1), parser.Resolve(line.Command.Identifier));
        Assert.Equal(new(3, 5), parser.Resolve(line.Command.Parameters[0]));
        Assert.Equal(new(3, 5), parser.Resolve(line.Command.Parameters[0].Value));
        Assert.Equal(new(3, 5), parser.Resolve(line.Command.Parameters[0].Value[0]));
        Assert.Equal(new(3, 1), parser.Resolve((line.Command.Parameters[0].Value[0] as IdentifiedText)!.Text));
        Assert.Equal(new(4, 4), parser.Resolve((line.Command.Parameters[0].Value[0] as IdentifiedText)!.Id));
        Assert.Equal(new(6, 1), parser.Resolve((line.Command.Parameters[0].Value[0] as IdentifiedText)!.Id.Body));
        Assert.Equal(new(9, 6), parser.Resolve(line.Command.Parameters[1]));
        Assert.Equal(new(9, 1), parser.Resolve(line.Command.Parameters[1].Identifier));
        Assert.Equal(new(11, 4), parser.Resolve(line.Command.Parameters[1].Value));
        Assert.Equal(new(11, 1), parser.Resolve(line.Command.Parameters[1].Value[0]));
        Assert.Equal(new(12, 3), parser.Resolve(line.Command.Parameters[1].Value[1]));
        Assert.Equal(new(13, 1), parser.Resolve((line.Command.Parameters[1].Value[1] as Expression)!.Body));
        Assert.Equal(new(16, 2), parser.Resolve(line.Command.Parameters[2]));
        Assert.Equal(new(16, 1), parser.Resolve(line.Command.Parameters[2].Identifier));
        Assert.Equal(new(17, 1), parser.Resolve(line.Command.Parameters[2].Value));
        Assert.Equal(new(19, 2), parser.Resolve(line.Command.Parameters[3]));
        Assert.Equal(new(19, 1), parser.Resolve(line.Command.Parameters[3].Value));
        Assert.Equal(new(20, 1), parser.Resolve(line.Command.Parameters[3].Identifier));
    }

    [Fact]
    public void TextIsIdentified ()
    {
        parser.Parse("@c foo|#f|far{f} p:bar|#b|nya|#n|");
        Assert.Equal("foo", parser.Identifications["f"]);
        Assert.Equal("bar", parser.Identifications["b"]);
        Assert.Equal("nya", parser.Identifications["n"]);
    }

    [Fact]
    public void WhenUnclosedQuotesInValueAllFollowingContentIsParsedAsValue ()
    {
        var line = parser.Parse(@"@c "" ");
        Assert.Equal(@""" ", line.Command.Parameters[0].Value[0] as PlainText);
    }

    [Fact]
    public void EscapedEmptyContentIsParsedAsEmptyValue ()
    {
        var line = parser.Parse("""
                                @c ""
                                """);
        Assert.Empty(line.Command.Parameters[0].Value);
    }

    [Fact]
    public void UnwrapsWhitespaceInValue ()
    {
        var line = parser.Parse("""
                                @c "a \" c"
                                """);
        Assert.Equal(@"a "" c", line.Command.Parameters[0].Value[0] as PlainText);
    }

    [Fact]
    public void WhenMissingExpressionCloseErrorIsAddedButOtherContentIsParsed ()
    {
        var line = parser.Parse(@"@c \[x{x\[ }x\{xx\}""\\{");
        Assert.Equal(@"[x", line.Command.Parameters[0].Value[0] as PlainText);
        Assert.Equal(@"x\[ ", (line.Command.Parameters[0].Value[1] as Expression)!.Body);
        Assert.Equal(@"x{xx}""\", line.Command.Parameters[0].Value[2] as PlainText);
        parser.HasError(MissingExpressionBody);
    }

    [Fact]
    public void EscapedQuotesAreUnescaped ()
    {
        var line = parser.Parse("""
                                @c "x\"\, \"x { \" }x\\"
                                """);
        Assert.Equal("""x"\, "x """, line.Command.Parameters[0].Value[0] as PlainText);
        Assert.Equal(@" \"" ", (line.Command.Parameters[0].Value[1] as Expression)!.Body);
        Assert.Equal(@"x\", line.Command.Parameters[0].Value[2] as PlainText);
    }

    [Fact]
    public void PreviousEscapesAreRespected ()
    {
        var line = parser.Parse(@"@c \\{ \\\ }\\\[");
        Assert.Equal(@"\", line.Command.Parameters[0].Value[0] as PlainText);
        Assert.Equal(@" \\\ ", (line.Command.Parameters[0].Value[1] as Expression)!.Body);
        Assert.Equal(@"\[", line.Command.Parameters[0].Value[2] as PlainText);
    }

    [Fact]
    public void UnescapesEscapeSymbol ()
    {
        AssertValueParsed(
            """
            foo bar\
            """,
            """
            "foo bar\\"
            """);
        AssertValueParsed(
            """
            a=" \";b=" \"
            """,
            """
            "a=\" \\\";b=\" \\\""
            """);
        AssertValueParsed(
            """
            a=" \";b=" \"
            """,
            """
            a=" \\";b=" \\"
            """);
        AssertValueParsed(
            """
            a=" \\";b=" \\"
            """,
            """
            a=" \\\\";b=" \\\\"
            """);
    }

    [Fact]
    public void WhenEscapeSymbolNotProperlyEscapedStillParses ()
    {
        AssertValueParsed(
            """
            foo\bar
            """,
            """
            foo\bar
            """);
        AssertValueParsed(
            """
            foo -\- bar
            """,
            """
            "foo -\- bar"
            """);
        AssertValueParsed(
            """
            a=" \ ";b=" \ "
            """,
            """
            a=" \ ";b=" \ "
            """);
    }

    [Fact]
    public void WhenAllSpacesWrappedDoesntUnwrap ()
    {
        AssertValueParsed(
            """
            foo="bar baz"
            """,
            """
            foo="bar baz"
            """);
        AssertValueParsed(
            """
            a=" x ";b=" x "
            """,
            """
            a=" x ";b=" x "
            """);
    }

    [Fact]
    public void WhenWrappedWithoutSpacesValueIsStillUnwrapped ()
    {
        AssertValueParsed(
            """
            "x"
            """,
            """
            "\"x\""
            """);
    }

    [Fact]
    public void DoesntUnescapeQuotesInsideQuotesWhenValueIsNotWrapped ()
    {
        // Required to allow propagating escaped quotes to expression evaluator, eg:
        // @set foo="text \"quoted text\" text"
        // If we'd go with a more consistent rule to escape quotes inside quotes
        // without exceptions, including inside un-wrapped values, it'd require:
        // @set foo="text \\\"quoted text\\\" text"
        // — here we have to escape both the escape symbol and the inner quotes.

        AssertValueParsed(
            """
            x"x \" y \" x"x
            """,
            """
            x"x \" y \" x"x
            """);
        AssertValueParsed(
            """
            foo=" \"x\" ";bar="\"y\""
            """,
            """
            foo=" \"x\" ";bar="\"y\""
            """);
        AssertValueParsed(
            """
            a=" \" ";b="\""
            """,
            """
            a=" \" ";b="\""
            """);
    }

    private void AssertValueParsed (string expectedParsedValue, string inputSerializedValue)
    {
        var line = parser.Parse($"@c {inputSerializedValue}");
        Assert.Equal(expectedParsedValue, line.Command.Parameters[0].Value[0] as PlainText);
    }
}
