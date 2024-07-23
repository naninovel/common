using static Naninovel.PathUtil;

namespace Naninovel.Utilities.Test;

public class PathUtilTest
{
    [Fact]
    public void CanResolveScriptName ()
    {
        Assert.Equal("FOO", ResolveScriptName(@"C:\DIR\FOO.NANI"));
        Assert.Equal("foo", ResolveScriptName("/dir/foo.nani"));
        Assert.Equal("foo", ResolveScriptName("ws://foo.nani"));
        Assert.Equal("foo", ResolveScriptName("foo.nani"));
        Assert.Equal("foo", ResolveScriptName("foo"));
        Assert.Equal("foo.bar", ResolveScriptName("foo.bar.nani"));
    }
}
