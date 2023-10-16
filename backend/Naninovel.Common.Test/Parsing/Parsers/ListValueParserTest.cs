namespace Naninovel.Parsing.Test;

public class ListValueParserTest
{
    private readonly ListValueParser parser = new();

    [Fact]
    public void WhenValueIsNullOrEmptyEmptyListIsReturned ()
    {
        Assert.Empty(parser.Parse(null));
        Assert.Empty(parser.Parse(""));
    }

    [Fact]
    public void ListSplitIntoIndividualItems ()
    {
        var items = parser.Parse("foo,bar.nya");
        Assert.Equal(2, items.Length);
        Assert.Equal("foo", items[0]);
        Assert.Equal("bar.nya", items[1]);
    }

    [Fact]
    public void SkippedItemsAreNull ()
    {
        var items = parser.Parse(",foo,,bar,");
        Assert.Equal(5, items.Length);
        Assert.Null(items[0]);
        Assert.Null(items[2]);
        Assert.Null(items[4]);
    }

    [Fact]
    public void EscapedDelimiterIsUnescapedAndIncludedToItem ()
    {
        var items = parser.Parse("1\\,2,3");
        Assert.Equal(2, items.Length);
        Assert.Equal("1,2", items[0]);
        Assert.Equal("3", items[1]);
    }
}
