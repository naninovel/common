using System.Net.WebSockets;
using Bootsharp;
using Naninovel.Bridging;
using Naninovel.Metadata;
using static Naninovel.Bindings.JSLogger;

namespace Naninovel.Bindings.Bridging;

public partial class Bridging(ISerializer serializer, TimeSpan scanDelay, TimeSpan timeout)
{
    private readonly ServerFinder finder = new(serializer);
    private CancellationTokenSource? tcs;
    private int preferredPort;
    private Client? client;

    [JSInvokable]
    public async void ConnectToServerInLoop (int preferredPort)
    {
        BreakConnectionLoop();
        this.preferredPort = preferredPort;
        while (tcs is { IsCancellationRequested: false })
            try { await ConnectToServerAsync(tcs.Token); }
            catch (Exception e) { LogError($"Bridging error: {e.Message}"); }
    }

    [JSInvokable]
    public void BreakConnectionLoop ()
    {
        tcs?.Cancel();
        tcs?.Dispose();
        tcs = new CancellationTokenSource();
    }

    [JSInvokable]
    public void RequestGoto (string scriptName, int lineIndex)
    {
        var spot = new PlaybackSpot { ScriptName = scriptName, LineIndex = lineIndex };
        client?.Send(new GotoRequest { PlaybackSpot = spot });
    }

    [JSFunction] public partial void OnMetadataUpdated (Project metadata);
    [JSFunction] public partial void OnPlaybackStatusUpdated (PlaybackStatus status);

    private async Task ConnectToServerAsync (CancellationToken token)
    {
        await Task.Delay(scanDelay, token);
        using var client = CreateClient();
        if (await FindServerAsync() is not { } server) return;
        if (await TryConnectAsync(client, server.Port) is not { } status) return;
        await MaintainConnectionAsync(status);
    }

    private Client CreateClient ()
    {
        client = new Client(new NetClientTransport(), serializer);
        client.Subscribe<UpdateMetadata>(m => OnMetadataUpdated(m.Metadata));
        client.Subscribe<UpdatePlaybackStatus>(m => OnPlaybackStatusUpdated(m.PlaybackStatus));
        return client;
    }

    private async Task<ServerInfo?> FindServerAsync ()
    {
        var startPort = preferredPort;
        var endPort = startPort + 2;
        var servers = await finder.FindServersAsync(startPort, endPort, timeout);
        return servers.FirstOrDefault();
    }

    private async Task<ConnectionStatus?> TryConnectAsync (Client client, int port)
    {
        using var cts = new CancellationTokenSource(timeout);
        try { return await client.ConnectAsync(port, cts.Token); }
        catch (WebSocketException) { return null; }
        catch (OperationCanceledException) { return null; }
    }

    private async Task MaintainConnectionAsync (ConnectionStatus status)
    {
        LogInfo($"Connected to {status.ServerInfo.Name} bridging server.");
        try { await status.MaintainTask; }
        catch (OperationCanceledException) { }
        catch (Exception e) { LogError($"Bridging maintain error: {e.Message}"); }
        finally { LogInfo($"Disconnected from {status.ServerInfo.Name} bridging server."); }
    }
}
