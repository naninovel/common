using Moq;

namespace Naninovel.Bridging.Test;

public class ServerTest
{
    private readonly MockServerTransport listener = new();
    private readonly MockSerializer serializer = new();
    private readonly Server server;

    public ServerTest ()
    {
        server = new Server("", listener, serializer);
    }

    [Fact]
    public void CanStart ()
    {
        server.Start(0);
        Assert.True(server.Listening);
    }

    [Fact]
    public void ListenedPortEqualStartPort ()
    {
        server.Start(7);
        Assert.Equal(7, server.ListenedPort);
    }

    [Fact]
    public async Task CanStop ()
    {
        server.Start(0);
        await server.Stop();
        Assert.False(server.Listening);
    }

    [Fact]
    public async Task CanStopWhileConnected ()
    {
        server.Start(0);
        await Connect(100);
        await server.Stop();
        Assert.False(server.Listening);
    }

    [Fact]
    public async Task CanRestart ()
    {
        server.Start(0);
        await server.Stop();
        server.Start(0);
        Assert.True(server.Listening);
    }

    [Fact]
    public void WhenStartingWhileStartedExceptionIsThrown ()
    {
        server.Start(0);
        Assert.Throws<InvalidOperationException>(() => server.Start(0));
    }

    [Fact]
    public async Task WhenDisposedWhileListeningExceptionIsThrown ()
    {
        await Connect();
        server.Dispose();
        await Assert.ThrowsAsync<ObjectDisposedException>(() => server.Stop());
    }

    [Fact]
    public async Task WhenDisposedWhileNotListeningExceptionIsNotThrown ()
    {
        await Connect();
        await server.Stop();
        server.Dispose();
        Assert.False(server.Listening);
    }

    [Fact]
    public async Task ConnectionCountIsEqualOpenConnections ()
    {
        var transports = await Connect(3);
        transports[0].Dispose();
        while (server.ConnectionsCount > 2)
            await Task.Yield();
        Assert.Equal(2, server.ConnectionsCount);
    }

    [Fact]
    public async Task WhenStoppedAllConnectionsRemoved ()
    {
        await Connect(100);
        await server.Stop();
        Assert.Equal(0, server.ConnectionsCount);
    }

    [Fact]
    public async Task WhenClientConnectedEventIsInvoked ()
    {
        var mre = new ManualResetEventSlim();
        server.OnClientConnected += _ => mre.Set();
        await Connect();
        await Task.Run(() => mre.Wait(TimeSpan.FromSeconds(1)));
        Assert.True(mre.IsSet);
    }

    [Fact]
    public async Task WhenClientDisconnectedEventIsInvoked ()
    {
        var mre = new ManualResetEventSlim();
        server.OnClientDisconnected += _ => mre.Set();
        (await Connect()).Dispose();
        await Task.Run(() => mre.Wait(TimeSpan.FromSeconds(1)));
        Assert.True(mre.IsSet);
    }

    [Fact]
    public async Task BroadcastMessageIsWrittenToSockets ()
    {
        var transports = await Connect(100);
        server.Broadcast(new ServerMessage());
        var messages = await Task.WhenAll(transports.Select(s => s.WaitOutcomingAsync<ServerMessage>()));
        Assert.All(messages, Assert.NotNull);
    }

    [Fact]
    public async Task AwaitedMessagesReturned ()
    {
        var transports = await Connect(100);
        var messages = await MockIncomingAsync<ClientMessage>(transports);
        Assert.All(messages, Assert.NotNull);
    }

    [Fact]
    public async Task AwaitedMessageReturnedAfterRestart ()
    {
        server.Start(0);
        var transport = await Connect();
        var message = await MockIncomingAsync<ClientMessage>(transport);
        await server.Stop();
        server.Start(0);
        transport = await Connect();
        message = await MockIncomingAsync<ClientMessage>(transport);
        Assert.NotNull(message);
        Assert.True(server.Listening);
    }

    [Fact]
    public async Task SubscribedHandlerInvokedOnEachMessage ()
    {
        const int count = 100;
        var cde = new CountdownEvent(count);
        server.Subscribe<ClientMessage>(_ => cde.Signal());
        var transports = await Connect(count);
        await MockIncomingAsync<ClientMessage>(transports);
        await Task.Run(() => cde.Wait(TimeSpan.FromSeconds(1)));
        Assert.Equal(0, cde.CurrentCount);
    }

    [Fact]
    public async Task UnsubscribedHandlerNotInvoked ()
    {
        var handler = new Mock<Action<ClientMessage>>();
        server.Subscribe<ClientMessage>(handler.Object);
        server.Unsubscribe<ClientMessage>(handler.Object);
        var transport = await Connect();
        await MockIncomingAsync<ClientMessage>(transport);
        handler.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ConnectionExceptionHandlerIsInvoked ()
    {
        var mre = new ManualResetEventSlim();
        server.Subscribe<ClientMessage>(_ => throw new Exception());
        server.Start(0);
        server.OnClientException += (_, _) => mre.Set();
        var transport = await Connect();
        transport.MockIncoming(new ClientMessage());
        await Task.Run(() => mre.Wait(TimeSpan.FromSeconds(1)));
        Assert.True(mre.IsSet);
    }

    [Fact]
    public async Task WhenExceptionHandlerMissingExceptionIsIgnored ()
    {
        var mre = new ManualResetEventSlim();
        server.Subscribe<ClientMessage>(_ => throw new Exception());
        server.Start(0);
        server.OnClientDisconnected += _ => mre.Set();
        var transport = await Connect();
        transport.MockIncoming(new ClientMessage());
        await Task.Run(() => mre.Wait(TimeSpan.FromSeconds(1)));
        Assert.True(mre.IsSet);
    }

    [Fact]
    public async Task CanSendDirectlyToConnection ()
    {
        server.OnClientConnected += c => c.Send(new ServerMessage());
        var transport = await Connect();
        Assert.NotNull(await transport.WaitOutcomingAsync<ServerMessage>());
    }

    [Fact]
    public async Task NonMessageDataIsIgnored ()
    {
        var transport = await Connect();
        transport.MockIncoming(string.Empty);
        Assert.NotNull(await MockIncomingAsync<ClientMessage>(transport));
    }

    private async Task<MockTransport> Connect ()
    {
        if (!server.Listening) server.Start(0);
        var transport = new MockTransport { Open = true };
        listener.MockIncomingConnection(transport);
        await transport.WaitOutcomingAsync<ConnectionAccepted>();
        return transport;
    }

    private Task<MockTransport[]> Connect (int count)
    {
        var tasks = new List<Task<MockTransport>>();
        for (int i = 0; i < count; i++)
            tasks.Add(Connect());
        return Task.WhenAll(tasks);
    }

    private async Task<T> MockIncomingAsync<T> (MockTransport transport)
        where T : IClientMessage, new()
    {
        var messages = await MockIncomingAsync<T>(new[] { transport });
        return messages[0];
    }

    private Task<T[]> MockIncomingAsync<T> (IEnumerable<MockTransport> transports)
        where T : IClientMessage, new()
    {
        var tasks = new List<Task<T>>();
        foreach (var transport in transports)
        {
            tasks.Add(server.Wait<T>());
            transport.MockIncoming(new T());
        }
        return Task.WhenAll(tasks);
    }
}
