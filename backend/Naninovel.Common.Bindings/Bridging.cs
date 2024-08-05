using System.Net.WebSockets;
using Bootsharp;
using Naninovel.Bridging;
using Naninovel.Metadata;
using static Naninovel.Bindings.JSLogger;

namespace Naninovel.Bindings;

public static partial class Bridging
{
    private static readonly TimeSpan scanDelay = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan timeout = TimeSpan.FromSeconds(1);
    private static readonly JsonSerializer serializer = new(Serializer.Options);
    private static readonly ServerFinder finder = new(serializer);
    private static CancellationTokenSource? tcs;
    private static int preferredPort;
    private static Client? client;

    [JSInvokable]
    public static async Task ConnectToServerInLoop (int preferredPort)
    {
        BreakConnectionLoop();
        Bridging.preferredPort = preferredPort;
        while (tcs is { IsCancellationRequested: false })
            try { await ConnectToServerAsync(tcs.Token); }
            catch (Exception e) { LogError($"Bridging error: {e.Message}"); }
    }

    [JSInvokable]
    public static void BreakConnectionLoop ()
    {
        tcs?.Cancel();
        tcs?.Dispose();
        tcs = new CancellationTokenSource();
    }

    [JSInvokable]
    public static void RequestGoto (string scriptPath, int lineIndex)
    {
        var spot = new PlaybackSpot { ScriptPath = scriptPath, LineIndex = lineIndex };
        client?.Send(new GotoRequest { PlaybackSpot = spot });
    }

    [JSFunction] public static partial void OnMetadataUpdated (Project metadata);
    [JSFunction] public static partial void OnPlaybackStatusUpdated (PlaybackStatus status);

    private static async Task ConnectToServerAsync (CancellationToken token)
    {
        await Task.Delay(scanDelay, token);
        using var client = CreateClient();
        if (await FindServerAsync() is not { } server) return;
        if (await TryConnectAsync(client, server.Port) is not { } status) return;
        await MaintainConnectionAsync(status);
    }

    private static Client CreateClient ()
    {
        client = new Client(new NetClientTransport(), serializer);
        client.Subscribe<UpdateMetadata>(m => OnMetadataUpdated(m.Metadata));
        client.Subscribe<UpdatePlaybackStatus>(m => OnPlaybackStatusUpdated(m.PlaybackStatus));
        return client;
    }

    private static async Task<ServerInfo?> FindServerAsync ()
    {
        var startPort = preferredPort;
        var endPort = startPort + 2;
        var servers = await finder.FindServersAsync(startPort, endPort, timeout);
        return servers.FirstOrDefault();
    }

    private static async Task<ConnectionStatus?> TryConnectAsync (Client client, int port)
    {
        using var cts = new CancellationTokenSource(timeout);
        try { return await client.ConnectAsync(port, cts.Token); }
        catch (WebSocketException) { return null; }
        catch (OperationCanceledException) { return null; }
    }

    private static async Task MaintainConnectionAsync (ConnectionStatus status)
    {
        LogInfo($"Connected to {status.ServerInfo.Name} bridging server.");
        try { await status.MaintainTask; }
        catch (OperationCanceledException) { }
        catch (Exception e) { LogError($"Bridging maintain error: {e.Message}"); }
        finally { LogInfo($"Disconnected from {status.ServerInfo.Name} bridging server."); }
    }
}
