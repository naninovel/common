namespace Naninovel.Parsing.Test;

public class ListValueSerializerTest
{
    private readonly ListValueSerializer serializer = new();

    [Fact]
    public void WhenEmptyReturnsEmpty ()
    {
        Assert.Equal("", serializer.Serialize(Array.Empty<string>()));
    }

    [Fact]
    public void CanSerializeListValue ()
    {
        Assert.Equal("foo,bar.nya", serializer.Serialize(new[] { "foo", "bar.nya" }));
    }

    [Fact]
    public void NullElementsAreEmpty ()
    {
        Assert.Equal("foo,,nya", serializer.Serialize(new[] { "foo", null, "nya" }));
        Assert.Equal(",,nya", serializer.Serialize(new[] { null, null, "nya" }));
    }

    [Fact]
    public void WhenAllElementsAreNullPreservesThem ()
    {
        Assert.Equal(",", serializer.Serialize(new string[] { null, null }));
    }

    [Fact]
    public void TrailingNullElementsArePreserved ()
    {
        Assert.Equal("foo,,", serializer.Serialize(new[] { "foo", null, null }));
    }

    [Fact]
    public void DelimitersAreEscaped ()
    {
        Assert.Equal("foo\\,bar,nya", serializer.Serialize(new[] { "foo,bar", "nya" }));
    }

    [Fact]
    public void DelimitersPrecededBySlashArePreserved ()
    {
        Assert.Equal("foo\\,bar,nya", serializer.Serialize(new[] { "foo\\,bar", "nya" }));
    }
}
