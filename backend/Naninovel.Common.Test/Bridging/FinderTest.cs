using System;
using System.Threading.Tasks;
using Xunit;

namespace Naninovel.Bridging.Test;

public class FinderTest
{
    private readonly TimeSpan timeout = TimeSpan.FromSeconds(1);

    [Fact]
    public async Task CanFindServer ()
    {
        var serverTransport = new MockServerTransport();
        var finder = new ServerFinder(() => {
            var clientTransport = new MockClientTransport();
            clientTransport.MockServers.Add(serverTransport);
            return clientTransport;
        });
        new Server("Foo", serverTransport).Start(1);
        var servers = await finder.FindServersAsync(1, 1, timeout);
        Assert.Equal("Foo", servers[0].Name);
        Assert.Equal(1, servers[0].Port);
    }

    [Fact]
    public async Task IgnoresOtherServices ()
    {
        var serverTransport1 = new MockServerTransport { ThrowOnConnection = true };
        var serverTransport2 = new MockServerTransport();
        var finder = new ServerFinder(() => {
            var clientTransport = new MockClientTransport();
            clientTransport.MockServers.Add(serverTransport1);
            clientTransport.MockServers.Add(serverTransport2);
            return clientTransport;
        });
        new Server("Other", serverTransport1).Start(1);
        new Server("MyBoy", serverTransport2).Start(2);
        var servers = await finder.FindServersAsync(1, 2, timeout);
        Assert.Equal("MyBoy", servers[0].Name);
        Assert.Equal(2, servers[0].Port);
    }

    [Fact]
    public async Task CanConstructWithDefaultTransport ()
    {
        var finder = new ServerFinder();
        Assert.Empty(await finder.FindServersAsync(0, 0, TimeSpan.Zero));
    }
}
