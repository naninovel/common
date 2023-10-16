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
        var param1 = new Parameter(new IValueComponent[] {
            new PlainText(@"\{}"),
            new Expression("x < y"),
            new PlainText(@"\")
        });
        var param2 = new Parameter("p", new[] { new PlainText(@"x "" { } [ ] \") });
        var command = new Command("cmd", new[] { param1, param2 });
        var line = new CommandLine(command);
        Assert.Equal(@"@cmd \\\{\}{x < y}\\ p:""x \"" \{ \} \[ \] \\""", serializer.Serialize(line));
    }

    [Fact]
    public void NamelessParameterIsWrittenFirst ()
    {
        var named = new Parameter("p", new[] { new PlainText("v") });
        var nameless = new Parameter(new[] { new PlainText("foo") });
        var command = new Command("cmd", new[] { named, nameless });
        Assert.Equal("@cmd foo p:v", serializer.Serialize(new CommandLine(command)));
    }

    [Fact]
    public void WhenStartingWithQuoteParameterValueIsWrapped ()
    {
        var param = new Parameter(new[] { new PlainText("\"") });
        var command = new Command("c", new[] { param });
        Assert.Equal(@"@c ""\""""", serializer.Serialize(new CommandLine(command)));
    }

    [Fact]
    public void WhenParameterValueContainQuotesItIsWrapped ()
    {
        var param = new Parameter(new[] { new PlainText("x\"x") });
        var command = new Command("c", new[] { param });
        Assert.Equal(@"@c ""x\""x""", serializer.Serialize(new CommandLine(command)));
    }

    [Fact]
    public void CanSerializeGenericLine ()
    {
        var prefix = new GenericPrefix("auth", "app");
        var inlined1 = new InlinedCommand(new("i"));
        var inlined2 = new InlinedCommand(new("cmd", new[] { new Parameter("p", new[] { new PlainText("v") }) }));
        var text1 = new MixedValue(new IValueComponent[] {
            new PlainText(@"""{}"),
            new Expression("x < y"),
            new PlainText(@"[]\ ")
        });
        var text2 = new MixedValue(new[] { new PlainText("x") });
        var line = new GenericLine(prefix, new IGenericContent[] { text1, inlined1, inlined2, text2 });
        Assert.Equal(@"auth.app: ""\{\}{x < y}\[\]\\ [i][cmd p:v]x", serializer.Serialize(line));
    }

    [Fact]
    public void WhenStartingWithQuoteGenericTextIsNotWrapped ()
    {
        var text = new MixedValue(new[] { new PlainText("\"") });
        var line = new GenericLine(new[] { text });
        Assert.Equal(@"""", serializer.Serialize(line));
    }

    [Fact]
    public void WhenGenericTextContainQuotesItIsNotWrapped ()
    {
        var text = new MixedValue(new[] { new PlainText("x\"x") });
        var line = new GenericLine(new[] { text });
        Assert.Equal("x\"x", serializer.Serialize(line));
    }

    [Fact]
    public void CanSerializeGenericLineWithoutPrefix ()
    {
        var line = new GenericLine(new[] { new MixedValue(new[] { new PlainText("x") }) });
        Assert.Equal("x", serializer.Serialize(line));
    }

    [Fact]
    public void CanSerializeGenericLineWithoutAppearance ()
    {
        var line = new GenericLine(new GenericPrefix("k"), new[] { new MixedValue(new[] { new PlainText("x") }) });
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
        Assert.Equal(@"""\{\}{x < y}\|#|#id||#|\[\]\\ ", serializer.Serialize(new IValueComponent[] {
            new PlainText(@"""{}"),
            new Expression("x < y"),
            new IdentifiedText("|#", new("id")),
            new IdentifiedText("", new("")),
            new PlainText(@"[]\ ")
        }, new() { ParameterValue = false, FirstGenericContent = false }));
        Assert.Equal(@"""\""\{\}{x < y}\[\]\\ """, serializer.Serialize(new IValueComponent[] {
            new PlainText(@"""{}"),
            new Expression("x < y"),
            new PlainText(@"[]\ ")
        }, new() { ParameterValue = true, FirstGenericContent = false }));
    }

    [Fact]
    public void WhenStartingWithQuotesWithWrapEnabledValueIsWrapped ()
    {
        Assert.Equal(@"""\""x\""""", serializer.Serialize(new[] {
            new PlainText(@"""x""")
        }, new() { ParameterValue = true, FirstGenericContent = false }));
    }

    [Fact]
    public void WhenStartingWithQuotesWithWrapDisabledValueIsNotWrapped ()
    {
        Assert.Equal(@"""x""", serializer.Serialize(new[] {
            new PlainText(@"""x""")
        }, new() { ParameterValue = false, FirstGenericContent = false }));
    }

    [Fact]
    public void WrappedMixedIsSerializedCorrectly ()
    {
        Assert.Equal(@""" { x }\\\""\{ \"" \}\\\""{ "" } \{ x \} """, serializer.Serialize(new IValueComponent[] {
            new PlainText(@" "),
            new Expression(" x "),
            new PlainText(@"\""{ "" }\"""),
            new Expression(@" "" "),
            new PlainText(@" { x } ")
        }, new() { ParameterValue = true, FirstGenericContent = false }));
    }

    [Fact]
    public void EscapesEscapedQuotesCorrectly ()
    {
        Assert.Equal(@"""a=\"" \\\"" \"";b=\"" \\\"" \""""", serializer.Serialize(new IValueComponent[] {
            new PlainText(@"a="" \"" "";b="" \"" """)
        }, new() { ParameterValue = true, FirstGenericContent = false }));
    }

    [Fact]
    public void TrimsTrailingEmptyLines ()
    {
        Assert.Equal("foo\n", serializer.Serialize(new IScriptLine[] {
            new GenericLine(new[] { new MixedValue(new[] { new PlainText("foo") }) }),
            new GenericLine(Array.Empty<IGenericContent>()),
            new GenericLine(Array.Empty<IGenericContent>())
        }));
    }

    [Fact]
    public void CanSerializeCommandWithEmptyNamelessParameter ()
    {
        var line = new CommandLine(new("cmd", new[] { new Parameter(new[] { new PlainText("") }) }));
        Assert.Equal("@cmd ", serializer.Serialize(line));
    }

    [Fact]
    public void CanSerializeEscapedAuthorAssign ()
    {
        Assert.Equal("x\\: x:x :x\n", serializer.Serialize(new IScriptLine[] {
            new GenericLine(new[] { new MixedValue(new[] { new PlainText("x: x:x :x") }) }),
        }));
        Assert.Equal("x : x\n", serializer.Serialize(new IScriptLine[] {
            new GenericLine(new[] { new MixedValue(new[] { new PlainText("x : x") }) })
        }));
    }
}
