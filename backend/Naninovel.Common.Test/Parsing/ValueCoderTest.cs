using System.Collections.Generic;
using Xunit;
using static Naninovel.Parsing.ValueCoder;

namespace Naninovel.Parsing.Test;

public class ValueCoderTest
{
    [Fact]
    public void WhenDecodeValueIsNullOriginalValueIsReturned ()
    {
        Assert.Null(Decode(null));
    }

    [Fact]
    public void WhenEncodeValueIsNullOriginalValueIsReturned ()
    {
        Assert.Null(Encode(null));
    }

    [Fact]
    public void WhenWrapValueIsNullOriginalValueIsReturned ()
    {
        Assert.Null(Wrap(null));
    }

    [Fact]
    public void WhenWrapValueIsEmptyDoubleQuotesAreReturned ()
    {
        Assert.Equal("\"\"", Wrap(""));
    }

    [Fact]
    public void SingleQuotesDecodedIntoOriginalValue ()
    {
        const string value = @""" ";
        Assert.Equal(@""" ", Decode(value));
    }

    [Fact]
    public void DoubleQuotesDecodedIntoEmpty ()
    {
        const string value = @"""""";
        Assert.Equal(string.Empty, Decode(value));
    }

    [Fact]
    public void ValueDecodedCorrectly ()
    {
        const string value = @"\[x{x\[ }x \{xx\}""\\";
        Assert.Equal(@"[x{x\[ }x {xx}""\", Decode(value));
    }

    [Fact]
    public void ValueUnwrappedCorrectly ()
    {
        const string value = @"""x\""\, \""x { \"" }x""";
        Assert.Equal(@"x""\, ""x { \"" }x", Decode(value));
    }

    [Fact]
    public void WhenWrappedWithoutSpacesValueIsUnwrapped ()
    {
        const string value = @"""\""x\""""";
        Assert.Equal(@"""x""", Decode(value));
    }

    [Fact]
    public void WhenUnwrappedQuotesArePreserved ()
    {
        const string value = @"a="" "";b="" """;
        Assert.Equal(@"a="" "";b="" """, Decode(value));
    }

    [Fact]
    public void ValueEncodedCorrectly ()
    {
        const string value = @"{x}{x}""[x]x";
        var expressions = new[] { (3, 3) };
        Assert.Equal(@"\{x\}{x}""\[x\]x", Encode(value, expressions));
    }

    [Fact]
    public void ValueWithoutExpressionsEncodedCorrectly ()
    {
        const string value = @"{x}";
        Assert.Equal(@"\{x\}", Encode(value));
    }

    [Fact]
    public void EncodeValueWrappedCorrectly ()
    {
        const string value = @" { x }\""{ "" }\""{ "" } { x } ";
        var expressions = new[] { (1, 5), (15, 5) };
        Assert.Equal(@""" { x }\\\""\{ \"" \}\\\""{ "" } \{ x \} """,
            Encode(value, expressions));
    }

    [Fact]
    public void EncodeEscapesExpressionSymbols ()
    {
        const string value = @"{x}";
        Assert.Equal(@"\{x\}", Encode(value));
    }

    [Fact]
    public void WhenAllSpacesWrappedEncodeDoesntEscapeQuotes ()
    {
        const string value = @"a="" "";b="" """;
        Assert.Equal(@"a="" "";b="" """, Encode(value));
    }

    [Fact]
    public void EvenWhenAllSpacesWrappedWrapWraps ()
    {
        const string value = @"a="" "";b="" """;
        Assert.Equal(@"""a=\"" \"";b=\"" \""""", Wrap(value));
    }

    [Fact]
    public void WrapDoesntEscapeControlCharacter ()
    {
        const string value = @"{x}\[a]";
        Assert.Equal(@"""{x}\[a]""", Wrap(value));
    }

    [Fact]
    public void WrapDoesntEscapeQuotesInIgnoredRanges ()
    {
        const string value = @"{ x "" } "" ";
        var expressions = new[] { (0, 7) };
        Assert.Equal(@"""{ x "" } \"" """, Wrap(value, expressions));
    }

    [Fact]
    public void WrapDoesntEscapeAlreadyEscapedQuotes ()
    {
        const string value = @"\""a b\""";
        Assert.Equal(@"""\""a b\""""", Wrap(value));
    }

    [Fact]
    public void RelativeRangesCalculatedCorrectly ()
    {
        var genericText = new GenericText();
        genericText.StartIndex = 10;
        genericText.Length = 8;

        var expression1 = new LineText();
        expression1.StartIndex = 10;
        expression1.Length = 3;
        genericText.Expressions.Add(expression1);

        var expression2 = new LineText();
        expression2.StartIndex = 14;
        expression2.Length = 4;
        genericText.Expressions.Add(expression2);

        var ranges = new List<(int, int)>();
        genericText.Expressions.GetRelativeRanges(genericText, ranges);
        Assert.Equal(0, ranges[0].Item1);
        Assert.Equal(3, ranges[0].Item2);
        Assert.Equal(4, ranges[1].Item1);
        Assert.Equal(4, ranges[1].Item2);
    }

    [Fact]
    public void WhenSplitListValueIsNullOrEmptyEmptyListIsReturned ()
    {
        Assert.Empty(SplitList(null));
        Assert.Empty(SplitList(""));
    }

    [Fact]
    public void ListSplitIntoIndividualItems ()
    {
        var items = SplitList("foo,bar.nya");
        Assert.Equal(2, items.Count);
        Assert.Equal("foo", items[0]);
        Assert.Equal("bar.nya", items[1]);
    }

    [Fact]
    public void SkippedItemsAreNullInSplitList ()
    {
        var items = SplitList(",foo,,bar,");
        Assert.Equal(5, items.Count);
        Assert.Null(items[0]);
        Assert.Null(items[2]);
        Assert.Null(items[4]);
    }

    [Fact]
    public void EscapedDelimiterIsUnescapedAndIncludedToItemInSplitList ()
    {
        var items = SplitList("1\\,2,3");
        Assert.Equal(2, items.Count);
        Assert.Equal("1,2", items[0]);
        Assert.Equal("3", items[1]);
    }

    [Fact]
    public void WhenSplitNamedValueIsNullOrEmptyNameAndValueAreNull ()
    {
        Assert.Equal((null, null), SplitNamed(null));
        Assert.Equal((null, null), SplitNamed(""));
    }

    [Fact]
    public void NamedSplitIntoNameAndValue ()
    {
        var (name, value) = SplitNamed("foo.bar");
        Assert.Equal("foo", name);
        Assert.Equal("bar", value);
    }

    [Fact]
    public void SkippedNameIsNullInSplitNamed ()
    {
        var (name, value) = SplitNamed(".bar");
        Assert.Null(name);
        Assert.Equal("bar", value);
    }

    [Fact]
    public void SkippedValueWithDelimiterIsNullInSplitNamed ()
    {
        var (name, value) = SplitNamed("foo.");
        Assert.Equal("foo", name);
        Assert.Null(value);
    }

    [Fact]
    public void SkippedValueWithoutDelimiterIsNullInSplitNamed ()
    {
        var (name, value) = SplitNamed("foo");
        Assert.Equal("foo", name);
        Assert.Null(value);
    }

    [Fact]
    public void WhenOnlyDelimiterBothAreNullInSplitNamed ()
    {
        Assert.Equal((null, null), SplitNamed("."));
    }

    [Fact]
    public void WhenMultipleUnescapedDelimitersFirstIsUsedInSplitNamed ()
    {
        var (name, value) = SplitNamed("foo.12.5");
        Assert.Equal("foo", name);
        Assert.Equal("12.5", value);
    }

    [Fact]
    public void EscapedDelimiterIsUnescapedAndIncludedInSplitNamed ()
    {
        var (name, value) = SplitNamed("foo\\.12.5");
        Assert.Equal("foo.12", name);
        Assert.Equal("5", value);
    }
}