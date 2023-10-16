namespace Naninovel.ManagedText.Test;

public class ManagedTextRecordTest
{
    [Fact]
    public void PropertiesAreAssignedOnConstruction ()
    {
        var record = new ManagedTextRecord("key", "value", "comment");
        Assert.Equal("key", record.Key);
        Assert.Equal("value", record.Value);
        Assert.Equal("comment", record.Comment);
    }

    [Fact]
    public void OptionalPropertiesAreEmptyByDefault ()
    {
        var record = new ManagedTextRecord("key");
        Assert.Empty(record.Value);
        Assert.Empty(record.Comment);
    }

    [Fact]
    public void RecordsAreComparedByKeyOnly ()
    {
        Assert.Equal(new ManagedTextRecord("foo", "bar", "nya"), new("foo", "baz", "jaz"));
        Assert.NotEqual(new ManagedTextRecord("foo", "foo", "foo"), new("bar", "foo", "foo"));
    }

    [Fact]
    public void RecordsAreHashedCorrectly ()
    {
        Assert.Equal(0, new ManagedTextRecord(null).GetHashCode());
        Assert.Equal("foo".GetHashCode(), new ManagedTextRecord("foo").GetHashCode());
        Assert.NotEqual((object)new ManagedTextRecord(""), 0);
    }

    [Fact]
    public void RecordsAreStringifiedCorrectly ()
    {
        Assert.Equal("->", new ManagedTextRecord().ToString());
        Assert.Equal("key->", new ManagedTextRecord("key").ToString());
        Assert.Equal("key->value", new ManagedTextRecord("key", "value").ToString());
        Assert.Equal("key->value (comment)", new ManagedTextRecord("key", "value", "comment").ToString());
    }
}
