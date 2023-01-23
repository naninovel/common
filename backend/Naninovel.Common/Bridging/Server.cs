using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Naninovel.Bridging;

public class Server : IDisposable
{
    public event Action<Connection>? OnClientConnected;
    public event Action<Connection>? OnClientDisconnected;
    public event Action<Exception, Connection>? OnClientException;

    public string Name { get; }
    public bool Listening => listener.Listening;
    public int ListenedPort { get; private set; }
    public int ConnectionsCount => connections.Count;
    public Task? WaitForExit { get; private set; }

    private readonly IServerTransport listener;
    private readonly Connections connections = new();
    private readonly Subscriber subscriber = new();
    private readonly Waiter waiter = new();

    public Server (string name, IServerTransport listener)
    {
        Name = name;
        this.listener = listener;
    }

    public void Start (int port, CancellationToken token = default)
    {
        if (Listening) throw new InvalidOperationException("Already listening.");
        listener.StartListening(port);
        ListenedPort = port;
        WaitForExit = ListenConnectionsAsync(token);
    }

    public Task StopAsync (CancellationToken token = default)
    {
        var tasks = connections.Enumerate().Select(c => c.CloseAsync(token)
            .ContinueWith(_ => c.Dispose(), token)).ToList();
        connections.Clear();
        listener.StopListening();
        if (WaitForExit != null) tasks.Add(WaitForExit);
        return Task.WhenAll(tasks);
    }

    public void Broadcast (IServerMessage message)
    {
        foreach (var connection in connections.Enumerate())
            connection.Send(message);
    }

    public void Subscribe<T> (Action<T> handler) where T : IClientMessage
    {
        subscriber.Subscribe(handler);
    }

    public void Unsubscribe<T> (Action<T> handler) where T : IClientMessage
    {
        subscriber.Unsubscribe(handler);
    }

    public Task<T> WaitAsync<T> (CancellationToken token = default) where T : IClientMessage
    {
        return waiter.WaitAsync<T>(token);
    }

    public void Dispose ()
    {
        foreach (var connection in connections.Enumerate())
            connection.Dispose();
        listener.Dispose();
    }

    private async Task ListenConnectionsAsync (CancellationToken token)
    {
        while (listener.Listening)
            try { AcceptConnection(await listener.WaitConnectionAsync(token)); }
            catch (OperationCanceledException) { break; }
    }

    private void AcceptConnection (ITransport transport)
    {
        var connection = new Connection(transport, subscriber, waiter);
        connection.Send(new ConnectionAccepted { ServerName = Name });
        connections.Add(connection);
        MaintainConnection(connection);
        OnClientConnected?.Invoke(connection);
    }

    private async void MaintainConnection (Connection connection)
    {
        try { await connection.MaintainAsync(); }
        catch (Exception e) { OnClientException?.Invoke(e, connection); }
        finally
        {
            connections.Remove(connection);
            OnClientDisconnected?.Invoke(connection);
        }
    }
}
