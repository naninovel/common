using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Naninovel.Parsing.TokenType;
using static Naninovel.Parsing.ErrorType;
using static Naninovel.Parsing.ParsingErrors;

namespace Naninovel.Parsing.Test;

public class CommandLineParserTest : LineParserTest<CommandLineParser, CommandLine>
{
    protected override string ExampleLine => "@char k.Happy pos:{x},10 wait:false";

    [Fact]
    public void WhenMissingLineIdErrorIsAdded ()
    {
        Parse("");
        Assert.True(HasError(MissingLineId));
    }

    [Fact]
    public void WhenMissingCommandIdErrorIsAdded ()
    {
        Parse("@");
        Assert.True(HasError(MissingCommandId));
    }

    [Fact]
    public void WhenMissingParameterIdErrorIsAdded ()
    {
        Parse("@c :v");
        Assert.True(HasError(MissingParamId));
    }

    [Fact]
    public void WhenMissingParameterValueErrorIsAdded ()
    {
        Parse("@c p:");
        Assert.True(HasError(MissingParamValue));
    }

    [Fact]
    public void WhenMissingExpressionBodyErrorIsAdded ()
    {
        Parse("@c {}");
        Assert.True(HasError(MissingExpressionBody));
    }

    [Fact]
    public void WhenMultipleNamelessErrorIsAdded ()
    {
        Parse("@c x y:z w");
        Assert.True(HasError(MultipleNameless));
    }

    [Fact]
    public void WhenCommandTokensMissingErrorIsAdded ()
    {
        var tokens = new List<Token> { new(LineId, 0, 1) };
        var errors = new List<ParseError>();
        Parser.Parse("@", tokens, errors);
        Assert.Contains(MissingCommandTokens, errors.Select(e => e.Message));
    }

    [Fact]
    public void IndexesEvaluatedCorrectly ()
    {
        var line = Parse("@xx x x:x{xx}x");
        Assert.Equal(0, line.StartIndex);
        Assert.Equal(14, line.Length);
        Assert.Equal(1, line.Command.StartIndex);
        Assert.Equal(13, line.Command.Length);
        Assert.Equal(4, line.Command.Parameters[0].StartIndex);
        Assert.Equal(1, line.Command.Parameters[0].Length);
        Assert.Equal(4, line.Command.Parameters[0].Value.StartIndex);
        Assert.Equal(1, line.Command.Parameters[0].Value.Length);
        Assert.Equal(6, line.Command.Parameters[1].StartIndex);
        Assert.Equal(8, line.Command.Parameters[1].Length);
        Assert.Equal(6, line.Command.Parameters[1].Identifier.StartIndex);
        Assert.Equal(1, line.Command.Parameters[1].Identifier.Length);
        Assert.Equal(8, line.Command.Parameters[1].Value.StartIndex);
        Assert.Equal(6, line.Command.Parameters[1].Value.Length);
        Assert.Equal(9, line.Command.Parameters[1].Value.Expressions[0].StartIndex);
        Assert.Equal(4, line.Command.Parameters[1].Value.Expressions[0].Length);
    }

    [Fact]
    public void CommandLineParsed ()
    {
        var line = Parse(ExampleLine);
        Assert.Equal("char", line.Command.Identifier);
        Assert.True(line.Command.Parameters[0].Nameless);
        Assert.False(line.Command.Parameters[0].Value.Dynamic);
        Assert.Equal("k.Happy", line.Command.Parameters[0].Value);
        Assert.Equal("pos", line.Command.Parameters[1].Identifier);
        Assert.Equal("{x},10", line.Command.Parameters[1].Value);
        Assert.Equal("wait", line.Command.Parameters[2].Identifier);
        Assert.Equal("false", line.Command.Parameters[2].Value);
    }

    [Fact]
    public void CommandWithExpressionsParsed ()
    {
        var line = Parse("@c {x} x:x{y}x{z}");
        Assert.True(line.Command.Parameters[0].Value.Dynamic);
        Assert.Single(line.Command.Parameters[0].Value.Expressions);
        Assert.Equal("{x}", line.Command.Parameters[0].Value.Text);
        Assert.Equal("{x}", line.Command.Parameters[0].Value.Expressions[0]);
        Assert.True(line.Command.Parameters[1].Value.Dynamic);
        Assert.Equal(2, line.Command.Parameters[1].Value.Expressions.Count);
        Assert.Equal("x{y}x{z}", line.Command.Parameters[1].Value.Text);
        Assert.Equal("{y}", line.Command.Parameters[1].Value.Expressions[0]);
        Assert.Equal("{z}", line.Command.Parameters[1].Value.Expressions[1]);
    }

    [Fact]
    public void NamelessAfterNamedParsed ()
    {
        var line = Parse("@c x:y z");
        Assert.True(line.Command.Parameters[1].Nameless);
        Assert.Equal("z", line.Command.Parameters[1].Value);
    }
}
