namespace Naninovel.ManagedText.Test;

public class ManagedTextDocumentTest
{
    [Fact]
    public void CanBeConstructedFromRecordsCollection ()
    {
        Assert.Equal([new("foo")], new ManagedTextDocument([new("foo")]).Records);
        Assert.Empty(new ManagedTextDocument(Array.Empty<ManagedTextRecord>()).Records);
    }

    [Fact]
    public void HeaderIsEmptyByDefault ()
    {
        Assert.Empty(new ManagedTextDocument(Array.Empty<ManagedTextRecord>()).Header);
    }

    [Fact]
    public void HeaderIsAssigned ()
    {
        Assert.Equal("foo", new ManagedTextDocument(Array.Empty<ManagedTextRecord>(), "foo").Header);
    }

    [Fact]
    public void CanCheckIfContainsRecordByKey ()
    {
        var doc = new ManagedTextDocument([new("foo")]);
        Assert.True(doc.Contains("foo"));
        Assert.False(doc.Contains("bar"));
    }

    [Fact]
    public void CanGetByKey ()
    {
        var doc = new ManagedTextDocument([new("foo")]);
        Assert.Equal("foo", doc.Get("foo").Key);
    }

    [Fact]
    public void ThrowsWhenGettingUnknownRecord ()
    {
        Assert.Throws<KeyNotFoundException>(() => new ManagedTextDocument(Array.Empty<ManagedTextRecord>()).Get(""));
    }

    [Fact]
    public void CanTryGetByKey ()
    {
        var doc = new ManagedTextDocument([new("foo")]);
        Assert.False(doc.TryGet("bar", out var record));
        Assert.True(doc.TryGet("foo", out record));
        Assert.Equal("foo", record.Key);
    }
}
