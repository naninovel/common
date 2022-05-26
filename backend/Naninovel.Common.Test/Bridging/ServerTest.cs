using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Naninovel.Bridging.Test;

public class ServerTest
{
    private readonly MockServerTransport listener = new();
    private readonly Server server;

    public ServerTest ()
    {
        server = new Server("", listener);
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
        await server.StopAsync();
        Assert.False(server.Listening);
    }

    [Fact]
    public async Task CanStopWhileConnected ()
    {
        server.Start(0);
        await ConnectAsync(100);
        await server.StopAsync();
        Assert.False(server.Listening);
    }

    [Fact]
    public async Task CanRestart ()
    {
        server.Start(0);
        await server.StopAsync();
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
        await ConnectAsync();
        server.Dispose();
        await Assert.ThrowsAsync<ObjectDisposedException>(() => server.StopAsync());
    }

    [Fact]
    public async Task WhenDisposedWhileNotListeningExceptionIsNotThrown ()
    {
        await ConnectAsync();
        await server.StopAsync();
        server.Dispose();
        Assert.False(server.Listening);
    }

    [Fact]
    public async Task ConnectionCountIsEqualOpenConnections ()
    {
        var transports = await ConnectAsync(3);
        transports[0].Dispose();
        while (server.ConnectionsCount > 2)
            await Task.Yield();
        Assert.Equal(2, server.ConnectionsCount);
    }

    [Fact]
    public async Task WhenStoppedAllConnectionsRemoved ()
    {
        await ConnectAsync(100);
        await server.StopAsync();
        Assert.Equal(0, server.ConnectionsCount);
    }

    [Fact]
    public async Task WhenClientConnectedEventIsInvoked ()
    {
        var mre = new ManualResetEventSlim();
        server.OnClientConnected += _ => mre.Set();
        await ConnectAsync();
        await Task.Run(() => mre.Wait(TimeSpan.FromSeconds(1)));
        Assert.True(mre.IsSet);
    }

    [Fact]
    public async Task WhenClientDisconnectedEventIsInvoked ()
    {
        var mre = new ManualResetEventSlim();
        server.OnClientDisconnected += _ => mre.Set();
        (await ConnectAsync()).Dispose();
        await Task.Run(() => mre.Wait(TimeSpan.FromSeconds(1)));
        Assert.True(mre.IsSet);
    }

    [Fact]
    public async Task BroadcastMessageIsWrittenToSockets ()
    {
        var transports = await ConnectAsync(100);
        server.Broadcast(new ServerMessage());
        var messages = await Task.WhenAll(transports.Select(s => s.WaitOutcomingAsync<ServerMessage>()));
        Assert.All(messages, Assert.NotNull);
    }

    [Fact]
    public async Task AwaitedMessagesReturned ()
    {
        var transports = await ConnectAsync(100);
        var messages = await MockIncomingAsync<ClientMessage>(transports);
        Assert.All(messages, Assert.NotNull);
    }

    [Fact]
    public async Task AwaitedMessageReturnedAfterRestart ()
    {
        server.Start(0);
        var transport = await ConnectAsync();
        var message = await MockIncomingAsync<ClientMessage>(transport);
        await server.StopAsync();
        server.Start(0);
        transport = await ConnectAsync();
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
        var transports = await ConnectAsync(count);
        await MockIncomingAsync<ClientMessage>(transports);
        await Task.Run(() => cde.Wait(TimeSpan.FromSeconds(1)));
        Assert.Equal(0, cde.CurrentCount);
    }

    [Fact, ExcludeFromCodeCoverage]
    public async Task UnsubscribedHandlerNotInvoked ()
    {
        var message = default(ClientMessage);
        server.Subscribe<ClientMessage>(m => message = m);
        server.Unsubscribe<ClientMessage>(m => message = m);
        var transport = await ConnectAsync();
        await MockIncomingAsync<ClientMessage>(transport);
        Assert.Null(message);
    }

    [Fact]
    public async Task ConnectionExceptionHandlerIsInvoked ()
    {
        var mre = new ManualResetEventSlim();
        server.Subscribe<ClientMessage>(_ => throw new Exception());
        server.Start(0);
        server.OnClientException += (_, _) => mre.Set();
        var transport = await ConnectAsync();
        transport.MockIncoming(new ClientMessage());
        await Task.Run(() => mre.Wait(TimeSpan.FromSeconds(1)));
        Assert.True(mre.IsSet);
    }

    [Fact]
    public async Task CanSendDirectlyToConnection ()
    {
        server.OnClientConnected += c => c.Send(new ServerMessage());
        var transport = await ConnectAsync();
        Assert.NotNull(await transport.WaitOutcomingAsync<ServerMessage>());
    }

    [Fact]
    public async Task NonMessageDataIsIgnored ()
    {
        var transport = await ConnectAsync();
        transport.MockIncoming(string.Empty);
        Assert.NotNull(await MockIncomingAsync<ClientMessage>(transport));
    }

    private async Task<MockTransport> ConnectAsync ()
    {
        if (!server.Listening) server.Start(0);
        var transport = new MockTransport { Open = true };
        listener.MockIncomingConnection(transport);
        await transport.WaitOutcomingAsync<ConnectionAccepted>();
        return transport;
    }

    private Task<MockTransport[]> ConnectAsync (int count)
    {
        var tasks = new List<Task<MockTransport>>();
        for (int i = 0; i < count; i++)
            tasks.Add(ConnectAsync());
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
            tasks.Add(server.WaitAsync<T>());
            transport.MockIncoming(new T());
        }
        return Task.WhenAll(tasks);
    }
}
