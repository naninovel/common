using System.Collections.Generic;
using Xunit;
using System.Linq;
using static Naninovel.Parsing.TokenType;
using static Naninovel.Parsing.ErrorType;
using static Naninovel.Parsing.ParsingErrors;

namespace Naninovel.Parsing.Test;

public class CommandLineParserTest
{
    private readonly ParseTestHelper<CommandLine> parser = new(new CommandLineParser().Parse);

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
        var errors = new List<ParseError>();
        var parser = new CommandLineParser();
        parser.Parse("@", new[] { new Token(LineId, 0, 1) }, errors);
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
        Assert.Equal("char", line.Command.Identifier);
        Assert.True(line.Command.Parameters[0].Nameless);
        Assert.False(line.Command.Parameters[0].Dynamic);
        Assert.Equal("k.Happy", (line.Command.Parameters[0].Value[0] as PlainText)!.Text);
        Assert.Equal("pos", line.Command.Parameters[1].Identifier);
        Assert.Equal("x", (line.Command.Parameters[1].Value[0] as Expression)!.Text);
        Assert.Equal(",10", (line.Command.Parameters[1].Value[1] as PlainText)!.Text);
        Assert.Equal("wait", line.Command.Parameters[2].Identifier);
        Assert.Equal("false", (line.Command.Parameters[2].Value[0] as PlainText)!.Text);
    }

    [Fact]
    public void ArbitraryCommandWithExpressionsIsParsedCorrectly ()
    {
        var line = parser.Parse("@c {x} x:x{y}x{z}");
        Assert.True(line.Command.Parameters[0].Dynamic);
        Assert.Single(line.Command.Parameters[0].Value.OfType<Expression>());
        Assert.Equal("x", (line.Command.Parameters[0].Value[0] as Expression)!.Text);
        Assert.True(line.Command.Parameters[1].Dynamic);
        Assert.Equal(2, line.Command.Parameters[1].Value.OfType<Expression>().Count());
        Assert.Equal("x", (line.Command.Parameters[1].Value[0] as PlainText)!.Text);
        Assert.Equal("y", (line.Command.Parameters[1].Value[1] as Expression)!.Text);
        Assert.Equal("x", (line.Command.Parameters[1].Value[2] as PlainText)!.Text);
        Assert.Equal("z", (line.Command.Parameters[1].Value[3] as Expression)!.Text);
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
}
