using Xunit;
using static Naninovel.Parsing.ErrorType;

namespace Naninovel.Parsing.Test;

public class GenericLineParserTest
{
    private readonly ParseTestHelper<GenericLine> parser = new(new GenericLineParser().Parse);

    [Fact]
    public void WhenMissingInlinedCommandIdErrorIsAdded ()
    {
        parser.Parse("[]");
        Assert.True(parser.HasError(MissingCommandId));
    }

    [Fact]
    public void PartialInlinedCommandIsParsed ()
    {
        Assert.IsType<InlinedCommand>(parser.Parse("[").Content[0]);
        Assert.IsType<InlinedCommand>(parser.Parse("[]").Content[0]);
        Assert.IsType<InlinedCommand>(parser.Parse("x[").Content[1]);
        Assert.IsType<InlinedCommand>(parser.Parse("x[]x").Content[1]);
    }

    [Fact]
    public void WhenEmptyExpressionErrorIsAddedAndExpressionIsEmpty ()
    {
        var line = parser.Parse("{}");
        Assert.Equal("", ((line.Content[0] as GenericText)!.Text[0] as Expression)!.Text);
        Assert.True(parser.HasError(MissingExpressionBody));
    }

    [Fact]
    public void WhenMissingPrefixItsNull ()
    {
        Assert.Null(parser.Parse("x").Prefix);
        Assert.Empty(parser.Errors);
    }

    [Fact]
    public void WhenMissingAppearanceItsNull ()
    {
        Assert.Null(parser.Parse("x: y").Prefix.Appearance);
        Assert.Empty(parser.Errors);
    }

    [Fact]
    public void WhenDelimitedButMissingAppearanceErrorIsAdded ()
    {
        parser.Parse("x.: y");
        Assert.True(parser.HasError(MissingAppearance));
    }

    [Fact]
    public void ArbitraryGenericLineIsParsed ()
    {
        var line = parser.Parse("k.h: x[i] {y}.");
        Assert.Equal("k", line.Prefix.Author);
        Assert.Equal("h", line.Prefix.Appearance);
        Assert.Equal("x", ((line.Content[0] as GenericText)!.Text[0] as PlainText)!.Text);
        Assert.Equal("i", (line.Content[1] as InlinedCommand)?.Command.Identifier);
        Assert.Equal(" ", ((line.Content[2] as GenericText)!.Text[0] as PlainText)!.Text);
        Assert.Equal("y", ((line.Content[2] as GenericText)!.Text[1] as Expression)!.Text);
        Assert.Equal(".", ((line.Content[2] as GenericText)!.Text[2] as PlainText)!.Text);
    }

    [Fact]
    public void ArbitraryGenericLineWithExpressionsIsParsed ()
    {
        var line = parser.Parse("x{y}[z {w}]");
        Assert.Equal("x", ((line.Content[0] as GenericText)!.Text[0] as PlainText)!.Text);
        Assert.Equal("y", ((line.Content[0] as GenericText)!.Text[1] as Expression)!.Text);
        Assert.Equal("z", (line.Content[1] as InlinedCommand)?.Command.Identifier);
        Assert.Equal("w", ((line.Content[1] as InlinedCommand)?.Command.Parameters[0].Value[0] as Expression)!.Text);
    }

    [Fact]
    public void PlainTextIdDecoded ()
    {
        var line = parser.Parse(@""" \{ \} "" \[ \] \\""");
        Assert.Equal(@""" { } "" [ ] \""", ((line.Content[0] as GenericText)!.Text[0] as PlainText)!.Text);
    }

    [Fact]
    public void CurlyBracketsInPrefixAreParsedAsPlainText ()
    {
        // Required for backward compat with v1, where author ID can contain expressions.
        var line = parser.Parse(@"{name}.{appearance}: My favourite drink is {drink}!");
        Assert.Equal(@"{name}", line.Prefix.Author);
        Assert.Equal(@"{appearance}", line.Prefix.Appearance);
        Assert.Equal(@"My favourite drink is ", ((line.Content[0] as GenericText)!.Text[0] as PlainText)!.Text);
        Assert.Equal(@"drink", ((line.Content[0] as GenericText)!.Text[1] as Expression)!.Text);
        Assert.Equal(@"!", ((line.Content[0] as GenericText)!.Text[2] as PlainText)!.Text);
    }
}
