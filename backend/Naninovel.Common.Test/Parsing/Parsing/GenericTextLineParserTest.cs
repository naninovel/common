using Xunit;
using static Naninovel.Parsing.ErrorType;

namespace Naninovel.Parsing.Test;

public class GenericTextLineParserTest : LineParserTest<GenericTextLineParser, GenericTextLine>
{
    protected override string ExampleLine => "k.h: x[i] {y}.";

    [Fact]
    public void WhenMissingCommandIdErrorIsAdded ()
    {
        Parse("[]");
        Assert.True(HasError(MissingCommandId));
    }

    [Fact]
    public void PartialInlinedCommandParsed ()
    {
        Assert.IsType<InlinedCommand>(Parse("[").Content[0]);
        Assert.IsType<InlinedCommand>(Parse("[]").Content[0]);
        Assert.IsType<InlinedCommand>(Parse("x[").Content[1]);
        Assert.IsType<InlinedCommand>(Parse("x[]x").Content[1]);
    }

    [Fact]
    public void InlinedCommandPositionIsValid ()
    {
        var inlined = (InlinedCommand)Parse("x[]").Content[1];
        Assert.Equal(1, inlined.StartIndex);
        Assert.Equal(2, inlined.EndIndex);
    }

    [Fact]
    public void EmptyExpressionIsParsed ()
    {
        Assert.Equal("{}", (Parse("{}").Content[0] as GenericText)?.Expressions[0]);
    }

    [Fact]
    public void WhenMissingExpressionBodyErrorIsAdded ()
    {
        Parse("{}");
        Assert.True(HasError(MissingExpressionBody));
    }

    [Fact]
    public void WhenMissingAuthorIsEmpty ()
    {
        var line = Parse("x");
        Assert.Equal(string.Empty, line.Prefix.AuthorIdentifier);
    }

    [Fact]
    public void WhenMissingAppearanceIsEmpty ()
    {
        var line = Parse("x: y");
        Assert.Equal(string.Empty, line.Prefix.AuthorAppearance);
    }

    [Fact]
    public void WhenMissingAppearanceValueErrorIsAdded ()
    {
        Parse("x.: y");
        Assert.True(HasError(MissingAppearance));
    }

    [Fact]
    public void IndexesEvaluatedCorrectly ()
    {
        var line = Parse("x.x: [x x:x]x");
        Assert.Equal(0, line.StartIndex);
        Assert.Equal(13, line.Length);
        Assert.Equal(0, line.Prefix.AuthorIdentifier.StartIndex);
        Assert.Equal(1, line.Prefix.AuthorIdentifier.Length);
        Assert.Equal(2, line.Prefix.AuthorAppearance.StartIndex);
        Assert.Equal(1, line.Prefix.AuthorAppearance.Length);
        Assert.Equal(5, (line.Content[0] as InlinedCommand)?.StartIndex);
        Assert.Equal(7, (line.Content[0] as InlinedCommand)?.Length);
        Assert.Equal(8, (line.Content[0] as InlinedCommand)?.Command.Parameters[0].StartIndex);
        Assert.Equal(3, (line.Content[0] as InlinedCommand)?.Command.Parameters[0].Length);
        Assert.Equal(12, (line.Content[1] as GenericText)?.StartIndex);
        Assert.Equal(1, (line.Content[1] as GenericText)?.Length);
    }

    [Fact]
    public void GenericTextLineParsed ()
    {
        var line = Parse(ExampleLine);
        Assert.Equal("k", line.Prefix.AuthorIdentifier);
        Assert.Equal("h", line.Prefix.AuthorAppearance);
        Assert.Equal("x", (line.Content[0] as GenericText)?.Text);
        Assert.Equal("i", (line.Content[1] as InlinedCommand)?.Command.Identifier);
        Assert.Equal(" {y}.", (line.Content[2] as GenericText)?.Text);
    }

    [Fact]
    public void GenericWithExpressionsParsed ()
    {
        var line = Parse("x{y}[z {w}]");
        Assert.Equal("x{y}", (line.Content[0] as GenericText)?.Text);
        Assert.Equal("{y}", (line.Content[0] as GenericText)?.Expressions[0]);
        Assert.Equal("z", (line.Content[1] as InlinedCommand)?.Command.Identifier);
        Assert.Equal("{w}", (line.Content[1] as InlinedCommand)?.Command.Parameters[0].Value.Expressions[0]);
    }
}
