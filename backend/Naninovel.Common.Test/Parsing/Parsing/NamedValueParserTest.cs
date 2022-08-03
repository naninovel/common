using Xunit;

namespace Naninovel.Parsing.Test;

public class NamedValueParserTest
{
    private readonly NamedValueParser parser = new();

    [Fact]
    public void WhenSplitNamedValueIsNullOrEmptyNameAndValueAreNull ()
    {
        Assert.Equal((null, null), parser.Parse(null));
        Assert.Equal((null, null), parser.Parse(""));
    }

    [Fact]
    public void NamedSplitIntoNameAndValue ()
    {
        var (name, value) = parser.Parse("foo.bar");
        Assert.Equal("foo", name);
        Assert.Equal("bar", value);
    }

    [Fact]
    public void SkippedNameIsNullInSplitNamed ()
    {
        var (name, value) = parser.Parse(".bar");
        Assert.Null(name);
        Assert.Equal("bar", value);
    }

    [Fact]
    public void SkippedValueWithDelimiterIsNullInSplitNamed ()
    {
        var (name, value) = parser.Parse("foo.");
        Assert.Equal("foo", name);
        Assert.Null(value);
    }

    [Fact]
    public void SkippedValueWithoutDelimiterIsNullInSplitNamed ()
    {
        var (name, value) = parser.Parse("foo");
        Assert.Equal("foo", name);
        Assert.Null(value);
    }

    [Fact]
    public void WhenOnlyDelimiterBothAreNullInSplitNamed ()
    {
        Assert.Equal((null, null), parser.Parse("."));
    }

    [Fact]
    public void WhenMultipleUnescapedDelimitersFirstIsUsedInSplitNamed ()
    {
        var (name, value) = parser.Parse("foo.12.5");
        Assert.Equal("foo", name);
        Assert.Equal("12.5", value);
    }

    [Fact]
    public void EscapedDelimiterIsUnescapedAndIncludedInSplitNamed ()
    {
        var (name, value) = parser.Parse("foo\\.12.5");
        Assert.Equal("foo.12", name);
        Assert.Equal("5", value);
    }
}
