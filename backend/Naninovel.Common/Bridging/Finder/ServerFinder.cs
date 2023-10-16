namespace Naninovel.Bridging;

public class ServerFinder
{
    private readonly Func<IClientTransport> clientFactory;
    private readonly ISerializer serializer;

    public ServerFinder (ISerializer serializer, Func<IClientTransport>? clientFactory = null)
    {
        this.serializer = serializer;
        this.clientFactory = clientFactory ?? (() => new NetClientTransport());
    }

    public async Task<List<ServerInfo>> FindServersAsync (int startPort, int endPort, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        var servers = new List<ServerInfo>();
        var tasks = new List<Task>();
        for (int port = startPort; port <= endPort; port++)
            tasks.Add(TryAddAsync(port, cts.Token));
        await Task.WhenAll(tasks);
        return servers;

        async Task TryAddAsync (int port, CancellationToken token)
        {
            try { servers.Add(await GetInfoAsync(port, token)); }
            catch { return; }
        }
    }

    private async Task<ServerInfo> GetInfoAsync (int port, CancellationToken token)
    {
        using var client = new Client(clientFactory(), serializer);
        var status = await client.ConnectAsync(port, token);
        return status.ServerInfo;
    }
}
