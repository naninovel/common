using System;
using Xunit;

namespace Naninovel.Parsing.Test;

public class ScriptSerializerTest
{
    private readonly ScriptSerializer serializer = new();

    [Fact]
    public void CanSerializeCommentLine ()
    {
        var line = new CommentLine("foo");
        Assert.Equal("; foo", serializer.Serialize(line));
    }

    [Fact]
    public void CanSerializeLabelLine ()
    {
        var line = new LabelLine("foo");
        Assert.Equal("# foo", serializer.Serialize(line));
    }

    [Fact]
    public void CanSerializeCommandLine ()
    {
        var param1 = new Parameter(null, new IMixedValue[] {
            new PlainText(@"""{}"),
            new Expression("x < y"),
            new PlainText(@"\")
        });
        var param2 = new Parameter("p", new[] { new PlainText(@"x "" { } [ ] \") });
        var command = new Command("cmd", new[] { param1, param2 });
        var line = new CommandLine(command);
        Assert.Equal(@"@cmd ""\{\}{x < y}\\ p:""x \"" \{ \} \[ \] \\""", serializer.Serialize(line));
    }

    [Fact]
    public void NamelessParameterIsWrittenFirst ()
    {
        var named = new Parameter("p", new[] { new PlainText("v") });
        var nameless = new Parameter(null, new[] { new PlainText("foo") });
        var command = new Command("cmd", new[] { named, nameless });
        Assert.Equal("@cmd foo p:v", serializer.Serialize(new CommandLine(command)));
    }

    [Fact]
    public void CanSerializeGenericLine ()
    {
        var prefix = new GenericPrefix("auth", "app");
        var inlined1 = new InlinedCommand(new("i"));
        var inlined2 = new InlinedCommand(new("cmd", new[] { new Parameter("p", new[] { new PlainText("v") }) }));
        var text1 = new GenericText(new IMixedValue[] {
            new PlainText(@"""{}"),
            new Expression("x < y"),
            new PlainText(@"[]\ ")
        });
        var text2 = new GenericText(new[] { new PlainText("x") });
        var line = new GenericLine(prefix, new IGenericContent[] { text1, inlined1, inlined2, text2 });
        Assert.Equal(@"auth.app: ""\{\}{x < y}\[\]\\ [i][cmd p:v]x", serializer.Serialize(line));
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
        var line = new GenericLine(new GenericPrefix("k"), new[] { new GenericText(new[] { new PlainText("x") }) });
        Assert.Equal("k: x", serializer.Serialize(line));
    }

    [Fact]
    public void CanSerializeMultipleLines ()
    {
        var comment = new CommentLine("foo");
        var label = new LabelLine("bar");
        var command = new CommandLine(new("nya"));
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
            new Expression("x < y"),
            new PlainText(@"[]\ ")
        }, false));
        Assert.Equal(@"""\""\{\}{x < y}\[\]\\ """, serializer.Serialize(new IMixedValue[] {
            new PlainText(@"""{}"),
            new Expression("x < y"),
            new PlainText(@"[]\ ")
        }, true));
    }
}
