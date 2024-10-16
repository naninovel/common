namespace Naninovel.Parsing.Test;

public class ScriptSerializerTest
{
    private readonly ScriptSerializer serde = new(Syntax.Default);

    [Fact]
    public void CanSerializeCommentLine ()
    {
        var line = new CommentLine("foo");
        Assert.Equal("; foo", serde.Serialize(line));
    }

    [Fact]
    public void CanSerializeLabelLine ()
    {
        var line = new LabelLine("foo");
        Assert.Equal("# foo", serde.Serialize(line));
    }

    [Fact]
    public void CanSerializeCommandLine ()
    {
        var param1 = new Parameter(new IValueComponent[] {
            new PlainText(@"\{}"),
            new Expression("x < y"),
            new PlainText(@"\")
        });
        var param2 = new Parameter("p", new[] { new PlainText(@"x "" { } [ ] x") });
        var command = new Command("cmd", [param1, param2]);
        var line = new CommandLine(command);
        Assert.Equal(
            """
            @cmd \\\{\}{x < y}\\ p:"x \" \{ \} \[ \] x"
            """, serde.Serialize(line));
    }

    [Fact]
    public void NamelessParameterIsWrittenFirst ()
    {
        var named = new Parameter("p", new[] { new PlainText("v") });
        var nameless = new Parameter(new[] { new PlainText("foo") });
        var command = new Command("cmd", [named, nameless]);
        Assert.Equal("@cmd foo p:v", serde.Serialize(new CommandLine(command)));
    }

    [Fact]
    public void WhenStartingWithQuoteParameterValueIsWrapped ()
    {
        var param = new Parameter(new[] { new PlainText("\"") });
        var command = new Command("c", [param]);
        Assert.Equal(
            """
            @c "\""
            """, serde.Serialize(new CommandLine(command)));
    }

    [Fact]
    public void WhenParameterValueContainQuotesItIsWrapped ()
    {
        var param = new Parameter(new[] { new PlainText("x\"x") });
        var command = new Command("c", [param]);
        Assert.Equal(
            """
            @c "x\"x"
            """, serde.Serialize(new CommandLine(command)));
    }

    [Fact]
    public void CanSerializeGenericLine ()
    {
        var prefix = new GenericPrefix("auth", "app");
        var inlined1 = new InlinedCommand(new("i"));
        var inlined2 = new InlinedCommand(new("cmd", [new Parameter("p", new[] { new PlainText("v") })]));
        var text1 = new MixedValue([
            new PlainText(@"""{}"),
            new Expression("x < y"),
            new PlainText(@"[]\ ")
        ]);
        var text2 = new MixedValue([new PlainText("x")]);
        var line = new GenericLine(prefix, [text1, inlined1, inlined2, text2]);
        Assert.Equal(@"auth.app: ""\{\}{x < y}\[\]\\ [i][cmd p:v]x", serde.Serialize(line));
    }

    [Fact]
    public void WhenStartingWithQuoteGenericTextIsNotWrapped ()
    {
        var text = new MixedValue([new PlainText("\"")]);
        var line = new GenericLine([text]);
        Assert.Equal(@"""", serde.Serialize(line));
    }

    [Fact]
    public void WhenGenericTextContainQuotesItIsNotWrapped ()
    {
        var text = new MixedValue([new PlainText("x\"x")]);
        var line = new GenericLine([text]);
        Assert.Equal("x\"x", serde.Serialize(line));
    }

    [Fact]
    public void CanSerializeGenericLineWithoutPrefix ()
    {
        var line = new GenericLine([new MixedValue([new PlainText("x")])]);
        Assert.Equal("x", serde.Serialize(line));
    }

    [Fact]
    public void CanSerializeGenericLineWithoutAppearance ()
    {
        var line = new GenericLine(new GenericPrefix("k"), [new MixedValue([new PlainText("x")])]);
        Assert.Equal("k: x", serde.Serialize(line));
    }

    [Fact]
    public void CanSerializeMultipleLines ()
    {
        var comment = new CommentLine("foo");
        var label = new LabelLine("bar");
        var command = new CommandLine(new("nya"));
        var lines = new IScriptLine[] { comment, label, command };
        Assert.Equal("; foo\n# bar\n@nya\n", serde.Serialize(lines));
    }

    [Fact]
    public void WhenEmptyLinesReturnsEmptyString ()
    {
        Assert.Empty(serde.Serialize(Array.Empty<IScriptLine>()));
    }

    [Fact]
    public void CanSerializeMixedValue ()
    {
        Assert.Equal(@"""\{\}{x < y}\|#|#id||#|\[\]\\ ", serde.Serialize([
            new PlainText(@"""{}"),
            new Expression("x < y"),
            new IdentifiedText("|#", new("id")),
            new IdentifiedText("", new("")),
            new PlainText(@"[]\ ")
        ], new() { ParameterValue = false, FirstGenericContent = false }));
        Assert.Equal(@"""\""\{\}{x < y}\[\]\\ """, serde.Serialize([
            new PlainText(@"""{}"),
            new Expression("x < y"),
            new PlainText(@"[]\ ")
        ], new() { ParameterValue = true, FirstGenericContent = false }));
    }

    [Fact]
    public void WhenStartingWithQuotesWithWrapEnabledValueIsWrapped ()
    {
        Assert.Equal(@"""\""x\""""", serde.Serialize([
            new PlainText(@"""x""")
        ], new() { ParameterValue = true, FirstGenericContent = false }));
    }

    [Fact]
    public void WhenStartingWithQuotesWithWrapDisabledValueIsNotWrapped ()
    {
        Assert.Equal(@"""x""", serde.Serialize([
            new PlainText(@"""x""")
        ], new() { ParameterValue = false, FirstGenericContent = false }));
    }

    [Fact]
    public void WrappedMixedIsSerialized ()
    {
        Assert.Equal("""
                     " { x }\\\"\{ \" \}\\\"{ " } \{ x \} "
                     """, serde.Serialize([
            new PlainText(" "),
            new Expression(" x "),
            new PlainText(@"\""{ "" }\"""),
            new Expression(@" "" "),
            new PlainText(" { x } ")
        ], new() { ParameterValue = true, FirstGenericContent = false }));
    }

    [Fact]
    public void EscapesEscapedQuotesInWrapped ()
    {
        Assert.Equal("""
                     "a=\" \\\" \"; b=\" \\\" \""
                     """, serde.Serialize([
            new PlainText("""
                          a=" \" "; b=" \" "
                          """)
        ], new() { ParameterValue = true, FirstGenericContent = false }));
    }

    [Fact]
    public void TrimsTrailingEmptyLines ()
    {
        Assert.Equal("foo\n", serde.Serialize([
            new GenericLine([new MixedValue([new PlainText("foo")])]),
            new GenericLine(Array.Empty<IGenericContent>()),
            new GenericLine(Array.Empty<IGenericContent>())
        ]));
    }

    [Fact]
    public void CanSerializeCommandWithEmptyNamelessParameter ()
    {
        var line = new CommandLine(new("cmd", [new Parameter(new[] { new PlainText("") })]));
        Assert.Equal("@cmd ", serde.Serialize(line));
    }

    [Fact]
    public void CanSerializeEscapedAuthorAssign ()
    {
        Assert.Equal("x\\: x:x :x\n", serde.Serialize([
            new GenericLine([new MixedValue([new PlainText("x: x:x :x")])])
        ]));
        Assert.Equal("x : x\n", serde.Serialize([
            new GenericLine([new MixedValue([new PlainText("x : x")])])
        ]));
    }

    [Fact]
    public void BooleanParametersSerializedAsFlags ()
    {
        var param1 = new Parameter("p1", new[] { new PlainText("true") });
        var param2 = new Parameter("p2", new[] { new PlainText("false") });
        var command = new Command("c", [param1, param2]);
        var line = new CommandLine(command);
        Assert.Equal("@c p1! !p2", serde.Serialize(line));
    }

    [Fact]
    public void IndentsSerialized ()
    {
        var label = new LabelLine("foo");
        var comment = new CommentLine("bar", 1);
        var command = new CommandLine(new Command("baz"), -1);
        var generic = new GenericLine(new GenericPrefix("x"),
            [new MixedValue([new PlainText("nya")])], 2);
        Assert.Equal("# foo", serde.Serialize(label));
        Assert.Equal("    ; bar", serde.Serialize(comment));
        Assert.Equal("@baz", serde.Serialize(command));
        Assert.Equal("        x: nya", serde.Serialize(generic));
    }

    [Fact]
    public void EmptyInlinedSerialized ()
    {
        Assert.Equal("[] x\t[]", serde.Serialize(new GenericLine(null,
        [
            new InlinedCommand(new Command("")),
            new MixedValue([new PlainText(" x\t")]),
            new InlinedCommand(new Command(""))
        ])));
        Assert.Equal("    [] x\t[]", serde.Serialize(new GenericLine(null,
        [
            new InlinedCommand(new Command("")),
            new MixedValue([new PlainText(" x\t")]),
            new InlinedCommand(new Command(""))
        ], 1)));
        Assert.Equal("    x: [] x\t[]", serde.Serialize(new GenericLine(new GenericPrefix("x"),
        [
            new InlinedCommand(new Command("")),
            new MixedValue([new PlainText(" x\t")]),
            new InlinedCommand(new Command(""))
        ], 1)));
    }

    [Fact]
    public void EscapesEscapeSymbols ()
    {
        AssertValueSerialized(
            """
            "foo bar\\"
            """,
            """
            foo bar\
            """);
        AssertValueSerialized(
            """
            foo\\bar
            """,
            """
            foo\bar
            """);
        AssertValueSerialized(
            """
            "foo -\\- bar"
            """,
            """
            foo -\- bar
            """);
        AssertValueSerialized(
            """
            a=" \\ ";b=" \\ "
            """,
            """
            a=" \ ";b=" \ "
            """);
        AssertValueSerialized(
            """
            a=" \\\\";b=" \\\\"
            """,
            """
            a=" \\";b=" \\"
            """);
    }

    [Fact]
    public void DoesntWrapWhenPartsWithSpaceAreAlreadyWrapped ()
    {
        AssertValueSerialized(
            """
            foo="bar baz"
            """,
            """
            foo="bar baz"
            """);
        AssertValueSerialized(
            """
            foo=" bar ";baz=" nya "
            """,
            """
            foo=" bar ";baz=" nya "
            """);
    }

    [Fact]
    public void WrapsTheValueWhenUnclosedQuotes ()
    {
        AssertValueSerialized(
            """
            "a=\"x"
            """,
            """
            a="x
            """);
        AssertValueSerialized(
            """
            "a=\"x y"
            """,
            """
            a="x y
            """);
        AssertValueSerialized(
            """
            "a=\""
            """,
            """
            a="
            """);
        AssertValueSerialized(
            """
            "a=\" \\\";b=\" \\\""
            """,
            """
            a=" \";b=" \"
            """);
    }

    [Fact]
    public void DoesntEscapeQuotesInsideQuotesWhenValueIsNotWrapped ()
    {
        // Required to allow propagating escaped quotes to expression evaluator, eg:
        // @set foo="text \"quoted text\" text"
        // If we'd go with a more consistent rule to escape quotes inside quotes
        // without exceptions, including inside un-wrapped values, it'd require:
        // @set foo="text \\\"quoted text\\\" text"
        // — here we have to escape both the escape symbol and the inner quotes.

        AssertValueSerialized(
            """
            x"x \" y \" x"x
            """,
            """
            x"x \" y \" x"x
            """);
        AssertValueSerialized(
            """
            foo=" \"x\" ";bar="\"y\""
            """,
            """
            foo=" \"x\" ";bar="\"y\""
            """);
        AssertValueSerialized(
            """
            a=" \" ";b="\""
            """,
            """
            a=" \" ";b="\""
            """);
    }

    [Fact]
    public void WrapsWhenLeadingOrTrailingBang ()
    {
        // Required to distinguish values with bang from nameless boolean parameters, eg:
        // @choice yes!
        // — is this a 'yes' boolean param with 'true' value or "yes!" text value?

        AssertValueSerialized(
            """
            "foo!"
            """,
            """
            foo!
            """);
        AssertValueSerialized(
            """
            "!bar"
            """,
            """
            !bar
            """);
    }

    private void AssertValueSerialized (string expectedSerializedValue, string inputRawValue)
    {
        Assert.Equal(expectedSerializedValue, serde.Serialize([new PlainText(inputRawValue)],
            new() { ParameterValue = true, FirstGenericContent = false }));
    }
}
