namespace Naninovel.Parsing.Test;

public class NamedValueSerializerTest
{
    private readonly NamedValueSerializer serializer = new(Syntax.Default);

    [Fact]
    public void CanSerializeWhenBothNameAndValueAreDefined ()
    {
        Assert.Equal("foo.bar", serializer.Serialize("foo", "bar"));
    }

    [Fact]
    public void CanSerializeWhenOnlyNameIsDefined ()
    {
        Assert.Equal("foo", serializer.Serialize("foo", null));
    }

    [Fact]
    public void CanSerializeWhenOnlyValueIsDefined ()
    {
        Assert.Equal(".foo", serializer.Serialize(null, "foo"));
    }

    [Fact]
    public void ReturnsEmptyWhenNeitherNameNorValueAreDefined ()
    {
        Assert.Equal("", serializer.Serialize(null, null));
    }

    [Fact]
    public void EscapesNamedDelimiterInNameButNotInValue ()
    {
        Assert.Equal("foo\\.bar.nya.far", serializer.Serialize("foo.bar", "nya.far"));
    }

    [Fact]
    public void DelimitersPrecededBySlashArePreserved ()
    {
        Assert.Equal("foo\\.bar.nya\\.far", serializer.Serialize("foo\\.bar", "nya\\.far"));
    }
}
