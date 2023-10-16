namespace Naninovel.Bridging;

public class Client : IDisposable
{
    private readonly Connection connection;
    private readonly IClientTransport transport;
    private Task? maintainTask;

    public Client (IClientTransport transport, ISerializer serializer)
    {
        this.transport = transport;
        connection = new Connection(transport, serializer);
    }

    public async Task<ConnectionStatus> ConnectAsync (int port, CancellationToken token = default)
    {
        if (maintainTask != null) throw new InvalidOperationException("Already connected.");
        await transport.ConnectToServerAsync(port, token);
        var waitAcceptedTask = connection.WaitAsync<ConnectionAccepted>(token);
        maintainTask = connection.MaintainAsync();
        var acceptedMessage = await waitAcceptedTask;
        var serverInfo = new ServerInfo(acceptedMessage.ServerName, port);
        return new ConnectionStatus(maintainTask, serverInfo);
    }

    public void Send (IClientMessage message)
    {
        connection.Send(message);
    }

    public void Subscribe<T> (Action<T> handler) where T : IServerMessage
    {
        connection.Subscribe(handler);
    }

    public void Unsubscribe<T> (Action<T> handler) where T : IServerMessage
    {
        connection.Unsubscribe(handler);
    }

    public Task<T> WaitAsync<T> (CancellationToken token = default) where T : IServerMessage
    {
        return connection.WaitAsync<T>(token);
    }

    public void Dispose ()
    {
        connection.CloseAsync().ContinueWith(_ => connection.Dispose());
    }
}
