using Xunit;
using static Naninovel.Parsing.ErrorType;

namespace Naninovel.Parsing.Test;

public class GenericLineParserTest
{
    private readonly ParseTestHelper<GenericLine> parser = new((e, a) => new GenericLineParser(e, a).Parse);

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
        Assert.Equal("", ((line.Content[0] as MixedValue)![0] as Expression)!.Body);
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
    public void WhenDelimitedAndMissingAppearanceErrorIsAddedButAuthorIsParsed ()
    {
        var line = parser.Parse("x.: y");
        Assert.True(parser.HasError(MissingAppearance));
        Assert.Equal("x", line.Prefix.Author);
    }

    [Fact]
    public void ArbitraryGenericLineIsParsed ()
    {
        var line = parser.Parse("k.h: x[i] {y}.");
        Assert.Equal("k", line.Prefix.Author);
        Assert.Equal("h", line.Prefix.Appearance);
        Assert.Equal("x", (line.Content[0] as MixedValue)![0] as PlainText);
        Assert.Equal("i", (line.Content[1] as InlinedCommand)?.Command.Identifier);
        Assert.Equal(" ", (line.Content[2] as MixedValue)![0] as PlainText);
        Assert.Equal("y", ((line.Content[2] as MixedValue)![1] as Expression)!.Body);
        Assert.Equal(".", (line.Content[2] as MixedValue)![2] as PlainText);
    }

    [Fact]
    public void ArbitraryGenericLineWithExpressionsIsParsed ()
    {
        var line = parser.Parse("x{y}[z {w}]");
        Assert.Equal("x", (line.Content[0] as MixedValue)![0] as PlainText);
        Assert.Equal("y", ((line.Content[0] as MixedValue)![1] as Expression)!.Body);
        Assert.Equal("z", (line.Content[1] as InlinedCommand)?.Command.Identifier);
        Assert.Equal("w", ((line.Content[1] as InlinedCommand)?.Command.Parameters[0].Value[0] as Expression)!.Body);
    }

    [Fact]
    public void PlainTextIdDecoded ()
    {
        var line = parser.Parse(@""" \{ \} "" \[ \] \\""");
        Assert.Equal(@""" { } "" [ ] \""", (line.Content[0] as MixedValue)![0] as PlainText);
    }

    [Fact]
    public void PreviousEscapesAreRespected ()
    {
        var line = parser.Parse(@"\\{ \\\ }\\\[");
        Assert.Equal(@"\", (line.Content[0] as MixedValue)![0] as PlainText);
        Assert.Equal(@" \\\ ", ((line.Content[0] as MixedValue)![1] as Expression)!.Body);
        Assert.Equal(@"\[", (line.Content[0] as MixedValue)![2] as PlainText);
    }

    [Fact]
    public void CurlyBracketsInPrefixAreParsedAsPlainText ()
    {
        // Required for backward compat with v1, where prefix can contain expressions.
        var line = parser.Parse(@"{name}.{appearance}: My favourite drink is {drink}!");
        Assert.Equal(@"{name}", line.Prefix.Author);
        Assert.Equal(@"{appearance}", line.Prefix.Appearance);
        Assert.Equal(@"My favourite drink is ", (line.Content[0] as MixedValue)![0] as PlainText);
        Assert.Equal(@"drink", ((line.Content[0] as MixedValue)![1] as Expression)!.Body);
        Assert.Equal(@"!", (line.Content[0] as MixedValue)![2] as PlainText);
    }

    [Fact]
    public void RangesAreAssociatedCorrectly ()
    {
        var line = parser.Parse("k.h: x{e}[i {e}]");
        Assert.Equal(new(0, 5), parser.Resolve(line.Prefix));
        Assert.Equal(new(0, 1), parser.Resolve(line.Prefix.Author));
        Assert.Equal(new(2, 1), parser.Resolve(line.Prefix.Appearance));
        Assert.Equal(new(5, 4), parser.Resolve(line.Content[0] as MixedValue));
        Assert.Equal(new(5, 1), parser.Resolve((line.Content[0] as MixedValue)![0] as PlainText));
        Assert.Equal(new(6, 3), parser.Resolve((line.Content[0] as MixedValue)![1] as Expression));
        Assert.Equal(new(7, 1), parser.Resolve(((line.Content[0] as MixedValue)![1] as Expression)!.Body));
        Assert.Equal(new(9, 7), parser.Resolve(line.Content[1] as InlinedCommand));
        Assert.Equal(new(10, 5), parser.Resolve((line.Content[1] as InlinedCommand)!.Command));
        Assert.Equal(new(10, 1), parser.Resolve((line.Content[1] as InlinedCommand)!.Command.Identifier));
        Assert.Equal(new(12, 3), parser.Resolve((line.Content[1] as InlinedCommand)!.Command.Parameters[0]));
        Assert.Equal(new(12, 3), parser.Resolve((line.Content[1] as InlinedCommand)!.Command.Parameters[0].Value));
        Assert.Equal(new(12, 3), parser.Resolve((line.Content[1] as InlinedCommand)!.Command.Parameters[0].Value[0] as Expression));
        Assert.Equal(new(13, 1), parser.Resolve(((line.Content[1] as InlinedCommand)!.Command.Parameters[0].Value[0] as Expression)!.Body));
    }
}
