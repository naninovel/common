using Naninovel.Metadata;
using Naninovel.Parsing;

namespace Naninovel.TestUtilities.Test;

public class MetadataMockTest
{
    private readonly MetadataMock meta = new();

    [Fact]
    public void CanSetupCommandWithEndpoint ()
    {
        var resolver = new EndpointResolver(meta);
        meta.SetupCommandWithEndpoint("goto");
        Assert.True(resolver.TryResolve(new("goto", [new(new([new PlainText("foo.bar")]))]), out var endpoint));
        Assert.Equal("foo", endpoint.ScriptPath);
        Assert.Equal("bar", endpoint.Label);
    }
}
