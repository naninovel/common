namespace Naninovel.Bridging;

public class ServerFinder (ISerializer serializer, Func<IClientTransport>? clientFactory = null)
{
    private readonly Func<IClientTransport> clientFactory = clientFactory ?? (() => new NetClientTransport());

    public async Task<List<ServerInfo>> FindServers (int startPort, int endPort, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        var servers = new List<ServerInfo>();
        var tasks = new List<Task>();
        for (int port = startPort; port <= endPort; port++)
            tasks.Add(TryAdd(port, cts.Token));
        await Task.WhenAll(tasks);
        return servers;

        async Task TryAdd (int port, CancellationToken token)
        {
            try { servers.Add(await GetInfo(port, token)); }
            catch { return; }
        }
    }

    private async Task<ServerInfo> GetInfo (int port, CancellationToken token)
    {
        using var client = new Client(clientFactory(), serializer);
        var status = await client.Connect(port, token);
        return status.ServerInfo;
    }
}
