using System.Linq;
using Xunit;
using static Naninovel.Parsing.TokenType;
using static Naninovel.Parsing.ErrorType;
using static Naninovel.Parsing.ParsingErrors;

namespace Naninovel.Parsing.Test;

public class CommandLineParserTest
{
    private readonly ParseTestHelper<CommandLine> parser = new((e, a) => new CommandLineParser(e, a).Parse);

    [Fact]
    public void WhenLineIdIsMissingErrorIsAddedAndCommandIsEmpty ()
    {
        var line = parser.Parse("foo");
        Assert.Empty(line.Command.Identifier.Text);
        Assert.Empty(line.Command.Parameters);
        Assert.True(parser.HasError(MissingLineId));
    }

    [Fact]
    public void WhenMissingCommandIdErrorIsAddedAndCommandIsEmpty ()
    {
        var line = parser.Parse("@ ");
        Assert.Empty(line.Command.Identifier.Text);
        Assert.Empty(line.Command.Parameters);
        Assert.True(parser.HasError(MissingCommandId));
    }

    [Fact]
    public void WhenCommandTokensMissingErrorIsAdded ()
    {
        var errors = new ErrorCollector();
        var parser = new CommandLineParser(errors);
        parser.Parse("@", new[] { new Token(LineId, 0, 1) });
        Assert.Contains(MissingCommandTokens, errors.Select(e => e.Message));
    }

    [Fact]
    public void CanParseCommandIdentifier ()
    {
        var line = parser.Parse("@foo");
        Assert.Equal("foo", line.Command.Identifier.Text);
        Assert.Empty(line.Command.Parameters);
        Assert.Empty(parser.Errors);
    }

    [Fact]
    public void TrimsSpaceBeforeCommandIdentifier ()
    {
        var line = parser.Parse("@ \t foo");
        Assert.Equal("foo", line.Command.Identifier.Text);
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
        Assert.Equal("p", line.Command.Parameters[0].Identifier.Text);
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
        Assert.False(line.Command.Parameters[0].Dynamic);
        Assert.True(line.Command.Parameters[1].Dynamic);
    }

    [Fact]
    public void ArbitraryCommandIsParsedCorrectly ()
    {
        var line = parser.Parse("@char k.Happy pos:{x},10 wait:false");
        Assert.Equal("char", line.Command.Identifier.Text);
        Assert.True(line.Command.Parameters[0].Nameless);
        Assert.False(line.Command.Parameters[0].Dynamic);
        Assert.Equal("k.Happy", (line.Command.Parameters[0].Value[0] as PlainText)!.Text);
        Assert.Equal("pos", line.Command.Parameters[1].Identifier.Text);
        Assert.Equal("x", (line.Command.Parameters[1].Value[0] as Expression)!.Body.Text);
        Assert.Equal(",10", (line.Command.Parameters[1].Value[1] as PlainText)!.Text);
        Assert.Equal("wait", line.Command.Parameters[2].Identifier.Text);
        Assert.Equal("false", (line.Command.Parameters[2].Value[0] as PlainText)!.Text);
    }

    [Fact]
    public void ArbitraryCommandWithExpressionsIsParsedCorrectly ()
    {
        var line = parser.Parse("@c {x} x:x{y}x{z}");
        Assert.True(line.Command.Parameters[0].Dynamic);
        Assert.Single(line.Command.Parameters[0].Value.OfType<Expression>());
        Assert.Equal("x", (line.Command.Parameters[0].Value[0] as Expression)!.Body.Text);
        Assert.True(line.Command.Parameters[1].Dynamic);
        Assert.Equal(2, line.Command.Parameters[1].Value.OfType<Expression>().Count());
        Assert.Equal("x", (line.Command.Parameters[1].Value[0] as PlainText)!.Text);
        Assert.Equal("y", (line.Command.Parameters[1].Value[1] as Expression)!.Body.Text);
        Assert.Equal("x", (line.Command.Parameters[1].Value[2] as PlainText)!.Text);
        Assert.Equal("z", (line.Command.Parameters[1].Value[3] as Expression)!.Body.Text);
    }

    [Fact]
    public void NamelessParameterAfterNamedParsed ()
    {
        var line = parser.Parse("@c x:y z");
        Assert.True(line.Command.Parameters[1].Nameless);
        Assert.Equal("z", (line.Command.Parameters[1].Value[0] as PlainText)!.Text);
    }

    [Fact]
    public void PlainTextIsDecoded ()
    {
        var line = parser.Parse(@"@c ""x \"" \{ \} \\"" p:""\{x\}\\");
        Assert.Equal(@"x "" { } \", (line.Command.Parameters[0].Value[0] as PlainText)!.Text);
        Assert.Equal(@"""{x}\", (line.Command.Parameters[1].Value[0] as PlainText)!.Text);
    }

    [Fact]
    public void WrapIsNotRequiredWhenInlineSpaceIsInQuotes ()
    {
        var line = parser.Parse(@"@set remark=""Shouting \""Stop the car!\"" was a mistake.""");
        Assert.Equal(@"remark=""Shouting \""Stop the car!\"" was a mistake.""",
            (line.Command.Parameters[0].Value[0] as PlainText)!.Text);
    }

    [Fact]
    public void SingleQuoteValueIsParsedCorrectly ()
    {
        var line = parser.Parse(@"@c ""\"""" p:v");
        Assert.Equal(@"""", (line.Command.Parameters[0].Value[0] as PlainText)!.Text);
    }

    [Fact]
    public void ExpressionWithQuotesIsParsedCorrectly ()
    {
        var line = parser.Parse(@"@c "" \"" { Random(var, "" \"" "") } \"" "" p:v");
        Assert.Equal(@" "" ", (line.Command.Parameters[0].Value[0] as PlainText)!.Text);
        Assert.Equal(@" Random(var, "" \"" "") ", (line.Command.Parameters[0].Value[1] as Expression)!.Body.Text);
        Assert.Equal(@" "" ", (line.Command.Parameters[0].Value[2] as PlainText)!.Text);
    }

    [Fact]
    public void RangesAreAssociatedCorrectly ()
    {
        var line = parser.Parse("@c v p:v{x}");
        Assert.Equal(new(1, 10), parser.Resolve(line.Command));
        Assert.Equal(new(1, 1), parser.Resolve(line.Command.Identifier));
        Assert.Equal(new(3, 1), parser.Resolve(line.Command.Parameters[0]));
        Assert.Equal(new(3, 1), parser.Resolve(line.Command.Parameters[0].Value[0] as PlainText));
        Assert.Equal(new(5, 6), parser.Resolve(line.Command.Parameters[1]));
        Assert.Equal(new(5, 1), parser.Resolve(line.Command.Parameters[1].Identifier));
        Assert.Equal(new(7, 1), parser.Resolve(line.Command.Parameters[1].Value[0] as PlainText));
        Assert.Equal(new(8, 3), parser.Resolve(line.Command.Parameters[1].Value[1] as Expression));
        Assert.Equal(new(9, 1), parser.Resolve((line.Command.Parameters[1].Value[1] as Expression)!.Body));
    }
}
