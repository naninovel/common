using Xunit;
using static Naninovel.Parsing.ErrorType;

namespace Naninovel.Parsing.Test;

public class GenericLineParserTest
{
    private readonly ParseTestHelper<GenericLine> parser = new(h => new GenericLineParser(h).Parse);

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
        Assert.Null(parser.Parse("x: y").Prefix?.Appearance);
        Assert.Empty(parser.Errors);
    }

    [Fact]
    public void WhenDelimitedAndMissingAppearanceErrorIsAddedButAuthorIsParsed ()
    {
        var line = parser.Parse("x.: y");
        Assert.True(parser.HasError(MissingAppearance));
        Assert.Equal("x", line.Prefix?.Author);
    }

    [Fact]
    public void GenericLineIsParsed ()
    {
        var line = parser.Parse("k.h: \"x[i] {y}.\"");
        Assert.Equal("k", line.Prefix?.Author);
        Assert.Equal("h", line.Prefix?.Appearance);
        Assert.Equal("\"x", (line.Content[0] as MixedValue)![0] as PlainText);
        Assert.Equal("i", (line.Content[1] as InlinedCommand)?.Command.Identifier);
        Assert.Equal(" ", (line.Content[2] as MixedValue)![0] as PlainText);
        Assert.Equal("y", ((line.Content[2] as MixedValue)![1] as Expression)!.Body);
        Assert.Equal(".\"", (line.Content[2] as MixedValue)![2] as PlainText);
    }

    [Fact]
    public void GenericLineWithExpressionsIsParsed ()
    {
        var line = parser.Parse("x{y}[z {w}]");
        Assert.Equal("x", (line.Content[0] as MixedValue)![0] as PlainText);
        Assert.Equal("y", ((line.Content[0] as MixedValue)![1] as Expression)!.Body);
        Assert.Equal("z", (line.Content[1] as InlinedCommand)?.Command.Identifier);
        Assert.Equal("w", ((line.Content[1] as InlinedCommand)?.Command.Parameters[0].Value[0] as Expression)!.Body);
    }

    [Fact]
    public void GenericLineWithIdentifiedTextIsParsed ()
    {
        var line = parser.Parse("x|#id1|{y}w|#id2|");
        Assert.Equal("x", ((IdentifiedText)(line.Content[0] as MixedValue)![0]).Text);
        Assert.Equal("id1", ((IdentifiedText)(line.Content[0] as MixedValue)![0]).Id.Body);
        Assert.Equal("y", ((Expression)(line.Content[0] as MixedValue)![1])!.Body);
        Assert.Equal("w", ((IdentifiedText)(line.Content[0] as MixedValue)![2]).Text);
        Assert.Equal("id2", ((IdentifiedText)(line.Content[0] as MixedValue)![2]).Id.Body);
    }

    [Fact]
    public void GenericLineWithUnclosedIdentifiedTextIsParsed ()
    {
        var line = parser.Parse("x|#id1");
        Assert.Equal("x", ((IdentifiedText)(line.Content[0] as MixedValue)![0]).Text);
        Assert.Equal("id1", ((IdentifiedText)(line.Content[0] as MixedValue)![0]).Id.Body);
    }

    [Fact]
    public void GenericLineWithEscapedIdentifiedTextIsParsed ()
    {
        var line = parser.Parse("x\\|#id1");
        Assert.Equal("x|#id1", (line.Content[0] as MixedValue)![0] as PlainText);
    }

    [Fact]
    public void PlainTextIdDecoded ()
    {
        var line = parser.Parse(@""" \{ \} "" \[ \] \|#| \\""");
        Assert.Equal(@""" { } "" [ ] |#| \""", (line.Content[0] as MixedValue)![0] as PlainText);
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
    public void ExpressionsInPrefixAreNotSupported ()
    {
        parser.Parse("{name}: My favourite drink is {drink}!");
        Assert.True(parser.HasError(ExpressionInGenericPrefix));
    }

    [Fact]
    public void RangesAreAssociatedCorrectly ()
    {
        var line = parser.Parse("k.h: x|#i|{e}[i {e}]");
        Assert.Equal(new(0, 5), parser.Resolve(line.Prefix));
        Assert.Equal(new(0, 1), parser.Resolve(line.Prefix?.Author));
        Assert.Equal(new(2, 1), parser.Resolve(line.Prefix?.Appearance));
        Assert.Equal(new(5, 8), parser.Resolve(line.Content[0] as MixedValue));
        Assert.Equal(new(5, 5), parser.Resolve((line.Content[0] as MixedValue)![0] as IdentifiedText));
        Assert.Equal(new(5, 1), parser.Resolve(((line.Content[0] as MixedValue)![0] as IdentifiedText)!.Text));
        Assert.Equal(new(6, 4), parser.Resolve(((line.Content[0] as MixedValue)![0] as IdentifiedText)!.Id));
        Assert.Equal(new(8, 1), parser.Resolve(((line.Content[0] as MixedValue)![0] as IdentifiedText)!.Id.Body));
        Assert.Equal(new(10, 3), parser.Resolve((line.Content[0] as MixedValue)![1] as Expression));
        Assert.Equal(new(11, 1), parser.Resolve(((line.Content[0] as MixedValue)![1] as Expression)!.Body));
        Assert.Equal(new(13, 7), parser.Resolve(line.Content[1] as InlinedCommand));
        Assert.Equal(new(14, 5), parser.Resolve((line.Content[1] as InlinedCommand)!.Command));
        Assert.Equal(new(14, 1), parser.Resolve((line.Content[1] as InlinedCommand)!.Command.Identifier));
        Assert.Equal(new(16, 3), parser.Resolve((line.Content[1] as InlinedCommand)!.Command.Parameters[0]));
        Assert.Equal(new(16, 3), parser.Resolve((line.Content[1] as InlinedCommand)!.Command.Parameters[0].Value));
        Assert.Equal(new(16, 3), parser.Resolve((line.Content[1] as InlinedCommand)!.Command.Parameters[0].Value[0] as Expression));
        Assert.Equal(new(17, 1), parser.Resolve(((line.Content[1] as InlinedCommand)!.Command.Parameters[0].Value[0] as Expression)!.Body));
    }

    [Fact]
    public void TextIsIdentifiedCorrectly ()
    {
        parser.Parse("k.h: foo|#f|far{f} [i bar|#b| p:nya|#n|]");
        Assert.Equal("foo", parser.Identifications["f"]);
        Assert.Equal("bar", parser.Identifications["b"]);
        Assert.Equal("nya", parser.Identifications["n"]);
    }

    [Fact]
    public void CanEscapeAuthorAssign ()
    {
        var line = parser.Parse(@"x\: x");
        Assert.Null(line.Prefix);
        Assert.Single(line.Content);
        Assert.Equal("x: x", (line.Content[0] as MixedValue)![0] as PlainText);
    }

    [Fact]
    public void CanEscapeSlashOverAuthorAssign ()
    {
        Assert.Equal(@"x\: x", (parser.Parse(@"x\\: x").Content[0] as MixedValue)![0] as PlainText);
    }
}
