namespace Naninovel.Parsing.Test;

public class NamedValueParserTest
{
    private readonly NamedValueParser parser = new(Identifiers.Default);

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
    public void SkippedNameIsNull ()
    {
        var (name, value) = parser.Parse(".bar");
        Assert.Null(name);
        Assert.Equal("bar", value);
    }

    [Fact]
    public void SkippedValueWithDelimiterIsNull ()
    {
        var (name, value) = parser.Parse("foo.");
        Assert.Equal("foo", name);
        Assert.Null(value);
    }

    [Fact]
    public void SkippedValueWithoutDelimiterIsNull ()
    {
        var (name, value) = parser.Parse("foo");
        Assert.Equal("foo", name);
        Assert.Null(value);
    }

    [Fact]
    public void WhenEmptyBothAreNull ()
    {
        Assert.Equal((null, null), parser.Parse(""));
    }

    [Fact]
    public void WhenOnlyDelimiterBothAreNull ()
    {
        Assert.Equal((null, null), parser.Parse("."));
    }

    [Fact]
    public void WhenMultipleUnescapedDelimitersFirstIsUsed ()
    {
        var (name, value) = parser.Parse("foo.12.5");
        Assert.Equal("foo", name);
        Assert.Equal("12.5", value);
    }

    [Fact]
    public void EscapedDelimiterIsUnescapedAndIncluded ()
    {
        var (name, value) = parser.Parse("foo\\.12.5");
        Assert.Equal("foo.12", name);
        Assert.Equal("5", value);
    }

    [Fact]
    public void EscapedDelimitersArePreserved ()
    {
        var (name, value) = parser.Parse("foo\\.12\\.5");
        Assert.Equal("foo\\.12\\.5", name);
        Assert.Null(value);
    }

    [Fact]
    public void EscapedDelimitersAfterActualDelimiterArePreserved ()
    {
        var (name, value) = parser.Parse("foo.12\\.5");
        Assert.Equal("foo", name);
        Assert.Equal("12\\.5", value);
    }
}
