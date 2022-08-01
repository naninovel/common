using System;
using Xunit;

namespace Naninovel.Parsing.Test;

public class ScriptSerializerTest
{
    private readonly ScriptSerializer serializer = new();

    [Fact]
    public void CanSerializeCommentLine ()
    {
        var line = new CommentLine(new("foo"));
        Assert.Equal("; foo", serializer.Serialize(line));
    }

    [Fact]
    public void CanSerializeLabelLine ()
    {
        var line = new LabelLine(new("foo"));
        Assert.Equal("# foo", serializer.Serialize(line));
    }

    [Fact]
    public void CanSerializeCommandLine ()
    {
        var param1 = new Parameter(new(new IMixedValue[] {
            new PlainText(@"\{}"),
            new Expression(new("x < y")),
            new PlainText(@"\")
        }));
        var param2 = new Parameter(new("p"), new(new[] { new PlainText(@"x "" { } [ ] \") }));
        var command = new Command(new("cmd"), new[] { param1, param2 });
        var line = new CommandLine(command);
        Assert.Equal(@"@cmd \\\{\}{x < y}\\ p:""x \"" \{ \} \[ \] \\""", serializer.Serialize(line));
    }

    [Fact]
    public void NamelessParameterIsWrittenFirst ()
    {
        var named = new Parameter(new("p"), new(new[] { new PlainText("v") }));
        var nameless = new Parameter(new(new[] { new PlainText("foo") }));
        var command = new Command(new("cmd"), new[] { named, nameless });
        Assert.Equal("@cmd foo p:v", serializer.Serialize(new CommandLine(command)));
    }

    [Fact]
    public void WhenStartingWithQuoteParameterValueIsWrapped ()
    {
        var param = new Parameter(new(new[] { new PlainText("\"") }));
        var command = new Command(new("c"), new[] { param });
        Assert.Equal(@"@c ""\""""", serializer.Serialize(new CommandLine(command)));
    }

    [Fact]
    public void WhenParameterValueContainQuotesItIsWrapped ()
    {
        var param = new Parameter(new(new[] { new PlainText("x\"x") }));
        var command = new Command(new("c"), new[] { param });
        Assert.Equal(@"@c ""x\""x""", serializer.Serialize(new CommandLine(command)));
    }

    [Fact]
    public void CanSerializeGenericLine ()
    {
        var prefix = new GenericPrefix(new("auth"), new("app"));
        var inlined1 = new InlinedCommand(new(new("i")));
        var inlined2 = new InlinedCommand(new(new("cmd"), new[] { new Parameter(new("p"), new(new[] { new PlainText("v") })) }));
        var text1 = new GenericText(new IMixedValue[] {
            new PlainText(@"""{}"),
            new Expression(new("x < y")),
            new PlainText(@"[]\ ")
        });
        var text2 = new GenericText(new[] { new PlainText("x") });
        var line = new GenericLine(prefix, new IGenericContent[] { text1, inlined1, inlined2, text2 });
        Assert.Equal(@"auth.app: ""\{\}{x < y}\[\]\\ [i][cmd p:v]x", serializer.Serialize(line));
    }

    [Fact]
    public void WhenStartingWithQuoteGenericTextIsNotWrapped ()
    {
        var text = new GenericText(new[] { new PlainText("\"") });
        var line = new GenericLine(new[] { text });
        Assert.Equal(@"""", serializer.Serialize(line));
    }

    [Fact]
    public void WhenGenericTextContainQuotesItIsNotWrapped ()
    {
        var text = new GenericText(new[] { new PlainText("x\"x") });
        var line = new GenericLine(new[] { text });
        Assert.Equal("x\"x", serializer.Serialize(line));
    }

    [Fact]
    public void CanSerializeGenericLineWithoutPrefix ()
    {
        var line = new GenericLine(new[] { new GenericText(new[] { new PlainText("x") }) });
        Assert.Equal("x", serializer.Serialize(line));
    }

    [Fact]
    public void CanSerializeGenericLineWithoutAppearance ()
    {
        var line = new GenericLine(new GenericPrefix(new("k")), new[] { new GenericText(new[] { new PlainText("x") }) });
        Assert.Equal("k: x", serializer.Serialize(line));
    }

    [Fact]
    public void CanSerializeMultipleLines ()
    {
        var comment = new CommentLine(new("foo"));
        var label = new LabelLine(new("bar"));
        var command = new CommandLine(new(new("nya")));
        var lines = new IScriptLine[] { comment, label, command };
        Assert.Equal("; foo\n# bar\n@nya\n", serializer.Serialize(lines));
    }

    [Fact]
    public void WhenEmptyLinesReturnsEmptyString ()
    {
        Assert.Empty(serializer.Serialize(Array.Empty<IScriptLine>()));
    }

    [Fact]
    public void CanSerializeMixedValue ()
    {
        Assert.Equal(@"""\{\}{x < y}\[\]\\ ", serializer.Serialize(new IMixedValue[] {
            new PlainText(@"""{}"),
            new Expression(new("x < y")),
            new PlainText(@"[]\ ")
        }, false));
        Assert.Equal(@"""\""\{\}{x < y}\[\]\\ """, serializer.Serialize(new IMixedValue[] {
            new PlainText(@"""{}"),
            new Expression(new("x < y")),
            new PlainText(@"[]\ ")
        }, true));
    }

    [Fact]
    public void WhenStartingWithQuotesWithWrapEnabledValueIsWrapped ()
    {
        Assert.Equal(@"""\""x\""""", serializer.Serialize(new[] { new PlainText(@"""x""") }, true));
    }

    [Fact]
    public void WhenStartingWithQuotesWithWrapDisabledValueIsNotWrapped ()
    {
        Assert.Equal(@"""x""", serializer.Serialize(new[] { new PlainText(@"""x""") }, false));
    }
}
