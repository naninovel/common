namespace Naninovel.Parsing.Test;

public class SemanticsTest
{
    [Fact]
    public void WhenParamIdentifierMissingNamelessIsTrue ()
    {
        Assert.True(new Parameter(Array.Empty<IValueComponent>()).Nameless);
    }

    [Fact]
    public void WhenParamIdentifierSpecifiedNamelessIsFalse ()
    {
        Assert.False(new Parameter("foo", Array.Empty<IValueComponent>()).Nameless);
    }

    [Fact]
    public void WhenParamValueContainsExpressionDynamicIsTrue ()
    {
        Assert.True(new Parameter(new[] { new Expression("") }).Value.Dynamic);
    }

    [Fact]
    public void WhenParamValueDoesntContainExpressionDynamicIsFalse ()
    {
        Assert.False(new Parameter(new[] { new PlainText("") }).Value.Dynamic);
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
        var param1 = new Parameter(new IValueComponent[] { new PlainText("v1"), new Expression("e") });
        var param2 = new Parameter("p2", new[] { new PlainText("v2") });
        var line = new CommandLine(new("c", new[] { param1, param2 }));
        Assert.Equal("@c v1{e} p2:v2", line.ToString());
    }

    [Fact]
    public void GenericLineToStringIsCorrect ()
    {
        var line = new GenericLine(new GenericPrefix("a", "b"), new IGenericContent[] {
            new MixedValue(new[] { new PlainText("x") }),
            new InlinedCommand(new("i", Array.Empty<Parameter>()))
        });
        Assert.Equal("a.b: x[i]", line.ToString());
    }

    [Fact]
    public void MixedValueToStringIsCorrect ()
    {
        Assert.Equal("foo|#id|{bar}nya", new MixedValue(new IValueComponent[] {
            new IdentifiedText(new("foo"), new(new("id"))),
            new Expression("bar"),
            new PlainText("nya")
        }).ToString());
    }

    [Fact]
    public void MixedValueCountEqualsItemCount ()
    {
        Assert.Equal(2, new MixedValue(new[] { new PlainText(""), new PlainText("") }).Count);
    }

    [Fact]
    public void MixedValueHasIndexerOverComponents ()
    {
        Assert.Equal("foo", new MixedValue(new[] { new PlainText("foo") })[0] as PlainText);
    }

    [Fact]
    public void MixedValueHasImplicitConversionFromArray ()
    {
        MixedValue mixed = Array.Empty<IValueComponent>();
        Assert.IsType<MixedValue>(mixed);
        Assert.NotNull(mixed);
    }

    [Fact]
    public void WhenConvertedFromNullArrayMixedValueIsNull ()
    {
        MixedValue mixed = (IValueComponent[])null;
        Assert.Null(mixed);
    }

    [Fact]
    public void PlainTextHasIndexerOverTextCharacters ()
    {
        Assert.Equal('1', new PlainText("012")[1]);
    }

    [Fact]
    public void PlainTextImplementsEnumerableOverTextCharacters ()
    {
        using var enumerator = new PlainText("012").GetEnumerator();
        enumerator.MoveNext();
        enumerator.MoveNext();
        Assert.Equal('1', enumerator.Current);
    }

    [Fact]
    public void PlainTextHasImplicitConversionToString ()
    {
        string @string = new PlainText("foo");
        Assert.IsType<string>(@string);
        Assert.Equal("foo", @string);
    }

    [Fact]
    public void WhenConvertedFromNullPlainTextStringIsNull ()
    {
        string @string = (PlainText)null;
        Assert.Null(@string);
    }

    [Fact]
    public void WhenConvertedFromEmptyPlainTextStringIsEmpty ()
    {
        string @string = PlainText.Empty;
        Assert.IsType<string>(@string);
        Assert.Empty(@string);
    }

    [Fact]
    public void PlainTextHasImplicitConversionFromString ()
    {
        PlainText plain = "foo";
        Assert.IsType<PlainText>(plain);
        Assert.Equal("foo", plain.Text);
    }

    [Fact]
    public void WhenConvertedFromNullStringPlainTextIsNull ()
    {
        PlainText plain = (string)null;
        Assert.Null(plain);
    }

    [Fact]
    public void WhenConvertedFromEmptyStringPlainTextIsEmpty ()
    {
        PlainText plain = string.Empty;
        Assert.Equal(PlainText.Empty, plain);
    }

    [Fact]
    public void IdentifiedTextToStringIsCorrect ()
    {
        Assert.Equal("text|#id|", new IdentifiedText(new("text"), new("id")).ToString());
    }

    [Fact]
    public void EmptyIdentifiedTextToStringIsCorrect ()
    {
        Assert.Equal("|#|", new IdentifiedText(new(""), new("")).ToString());
    }

    [Fact]
    public void WaitFlagIsNullByDefault ()
    {
        Assert.Null(new Command("").WaitFlag);
    }

    [Fact]
    public void WaitFlagImplicitlyConvertedToBool ()
    {
        #pragma warning disable xUnit2025
        Assert.True(new Command("", null, true).WaitFlag == true);
        Assert.True(new Command("", null, false).WaitFlag == false);
        Assert.False(new Command("").WaitFlag == true);
        Assert.False(new Command("").WaitFlag == false);
        Assert.Null(new Command("", null, default(bool?)).WaitFlag);
        #pragma warning restore xUnit2025
    }

    [Fact]
    public void WaitFlagToStringIsCorrect ()
    {
        Assert.Equal("c <", new Command("c", null, true).ToString());
        Assert.Equal("c >", new Command("c", null, false).ToString());
    }
}
