using System;
using Xunit;

namespace Naninovel.Parsing.Test;

public class SemanticsTest
{
    [Fact]
    public void WhenParamIdentifierMissingNamelessIsTrue ()
    {
        Assert.True(new Parameter(new(Array.Empty<IMixedValue>())).Nameless);
    }

    [Fact]
    public void WhenParamIdentifierSpecifiedNamelessIsFalse ()
    {
        Assert.False(new Parameter(new("foo"), new(Array.Empty<IMixedValue>())).Nameless);
    }

    [Fact]
    public void WhenParamValueContainsExpressionDynamicIsTrue ()
    {
        Assert.True(new Parameter(new(new[] { new Expression(PlainText.Empty) })).Value.Dynamic);
    }

    [Fact]
    public void WhenParamValueDoesntContainExpressionDynamicIsFalse ()
    {
        Assert.False(new Parameter(new(new[] { new PlainText("") })).Value.Dynamic);
    }

    [Fact]
    public void CommentLineToStringIsCorrect ()
    {
        var comment = new CommentLine(new("foo"));
        Assert.Equal("; foo", comment.ToString());
    }

    [Fact]
    public void LabelLineToStringIsCorrect ()
    {
        var line = new LabelLine(new("foo"));
        Assert.Equal("# foo", line.ToString());
    }

    [Fact]
    public void CommandLineToStringIsCorrect ()
    {
        var param1 = new Parameter(new(new IMixedValue[] { new PlainText("v1"), new Expression(new("e")) }));
        var param2 = new Parameter(new("p2"), new(new[] { new PlainText("v2") }));
        var line = new CommandLine(new(new("c"), new[] { param1, param2 }));
        Assert.Equal("@c v1{e} p2:v2", line.ToString());
    }

    [Fact]
    public void GenericLineToStringIsCorrect ()
    {
        var line = new GenericLine(new GenericPrefix(new("a"), new("b")), new IGenericContent[] {
            new GenericText(new[] { new PlainText(new("x")) }),
            new InlinedCommand(new(new("i"), Array.Empty<Parameter>()))
        });
        Assert.Equal("a.b: x[i]", line.ToString());
    }
}
