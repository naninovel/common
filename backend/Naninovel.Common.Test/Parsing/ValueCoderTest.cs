using Xunit;
using static Naninovel.Parsing.ValueCoder;

namespace Naninovel.Parsing.Test;

public class ValueCoderTest
{
    
   

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

    [Fact]
    public void IsEscapeDetectsEscapedSymbols ()
    {
        Assert.True(IsEscaped(@"\{", 1));
    }

    [Fact]
    public void IsEscapeDetectsPreviousEscapes ()
    {
        Assert.False(IsEscaped(@"\\{", 2));
    }
}
