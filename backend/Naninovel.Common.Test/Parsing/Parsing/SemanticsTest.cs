using System;
using Xunit;

namespace Naninovel.Parsing.Test;

public class SemanticsTest
{
    [Fact]
    public void WhenParamIdentifierMissingNamelessIsTrue ()
    {
        Assert.True(new Parameter(null, Array.Empty<IMixedValue>()).Nameless);
    }

    [Fact]
    public void WhenParamIdentifierSpecifiedNamelessIsFalse ()
    {
        Assert.False(new Parameter("foo", Array.Empty<IMixedValue>()).Nameless);
    }

    [Fact]
    public void WhenParamValueContainsExpressionDynamicIsTrue ()
    {
        Assert.True(new Parameter(null, new[] { new Expression("") }).Dynamic);
    }

    [Fact]
    public void WhenParamValueDoesntContainExpressionDynamicIsFalse ()
    {
        Assert.False(new Parameter(null, new[] { new PlainText("") }).Dynamic);
    }

    [Fact]
    public void CommentLineToStringIsCorrect ()
    {
        var comment = new CommentLine("foo");
        Assert.Equal("; foo", comment.ToString());
    }

    [Fact]
    public void LabelLineToStringIsCorrect ()
    {
        var line = new LabelLine("foo");
        Assert.Equal("# foo", line.ToString());
    }

    [Fact]
    public void CommandLineToStringIsCorrect ()
    {
        var param1 = new Parameter(null, new IMixedValue[] { new PlainText("v1"), new Expression("e") });
        var param2 = new Parameter("p2", new[] { new PlainText("v2") });
        var line = new CommandLine(new("c", new[] { param1, param2 }));
        Assert.Equal("@c v1{e} p2:v2", line.ToString());
    }

    [Fact]
    public void GenericLineToStringIsCorrect ()
    {
        var line = new GenericLine(new GenericPrefix("a", "b"), new IGenericContent[] {
            new GenericText(new[] { new PlainText("x") }),
            new InlinedCommand(new("i", Array.Empty<Parameter>()))
        });
        Assert.Equal("a.b: x[i]", line.ToString());
    }
}
