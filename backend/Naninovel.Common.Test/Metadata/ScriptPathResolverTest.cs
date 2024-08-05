namespace Naninovel.Metadata.Test;

public class ScriptPathResolverTest
{
    [Theory]
    [InlineData("/", "/foo.nani", "foo")]
    [InlineData("/", "/foo/bar.nani", "foo/bar")]
    [InlineData("/foo/", "/foo/bar.nani", "bar")]
    [InlineData("/foo", "/foo/bar.nani", "bar")]
    [InlineData("foo", "/foo/bar.nani", "bar")]
    [InlineData("foo/bar", "/foo/bar/nya.nani", "nya")]
    [InlineData("/", "foo", "foo")]
    [InlineData("", "foo", "foo")]
    [InlineData("\\foo/bar", "\\foo/bar\\nya.nani", "nya")]
    [InlineData("nya", "/foo/bar.nani", "foo/bar")]
    public void ResolveTheory (string rootUri, string fileUri, string expectedPath)
    {
        var resolver = new ScriptPathResolver { RootUri = rootUri };
        Assert.Equal(expectedPath, resolver.Resolve(fileUri));
    }

    [Fact]
    public void RootUriFormatted ()
    {
        Assert.Equal("foo/bar/", new ScriptPathResolver { RootUri = "foo\\bar" }.RootUri);
    }

    [Fact]
    public void MemoRespectsRoot ()
    {
        var resolver = new ScriptPathResolver { RootUri = "/foo" };
        Assert.Equal("bar/nya", resolver.Resolve("/foo/bar/nya.nani"));
        Assert.Equal("bar/nya", resolver.Resolve("/foo/bar/nya.nani"));
        resolver.RootUri = "/foo/bar";
        Assert.Equal("nya", resolver.Resolve("/foo/bar/nya.nani"));
    }
}
