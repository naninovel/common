using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Naninovel.Bridging.Test;

public class ClientTest
{
    private readonly MockClientTransport transport = new();
    private readonly Client client;

    public ClientTest ()
    {
        client = new Client(transport);
    }

    [Fact]
    public async Task CanConnect ()
    {
        var status = await ConnectAsync();
        Assert.True(status.Connected);
    }

    [Fact]
    public async Task ServerInformationIsCorrect ()
    {
        const string expectedName = nameof(ServerInformationIsCorrect);
        const int expectedPort = 666;
        var status = await ConnectAsync(expectedName, expectedPort);
        Assert.Equal(expectedName, status.ServerInfo.Name);
        Assert.Equal(expectedPort, status.ServerInfo.Port);
    }

    [Fact]
    public async Task WhenConnectingWhileConnectedExceptionIsThrown ()
    {
        await ConnectAsync();
        await Assert.ThrowsAsync<InvalidOperationException>(ConnectAsync);
    }

    [Fact]
    public async Task WhenCanceledWhileConnectingMethodReturns ()
    {
        using var cts = new CancellationTokenSource(1);
        await Assert.ThrowsAsync<TaskCanceledException>(() => client.ConnectAsync(0, cts.Token));
    }

    [Fact]
    public async Task WhenDisposedConnectionIsClosed ()
    {
        var status = await ConnectAsync();
        client.Dispose();
        await status.MaintainTask;
        Assert.False(status.Connected);
        Assert.False(transport.Open);
    }

    [Fact]
    public async Task SentMessageWrittenToSocket ()
    {
        await ConnectAsync();
        client.Send(new ClientMessage());
        var message = await transport.WaitOutcomingAsync<ClientMessage>();
        Assert.NotNull(message);
    }

    [Fact]
    public async Task WhenSocketClosedWhileSendingConnectionCloses ()
    {
        var status = await ConnectAsync();
        client.Send(new ClientMessage());
        transport.Open = false;
        await status.MaintainTask;
        Assert.False(status.Connected);
    }

    [Fact]
    public async Task AwaitedMessageReturned ()
    {
        await ConnectAsync();
        Assert.NotNull(await MockIncomingAsync<ServerMessage>());
    }

    [Fact]
    public async Task MultipleAwaitedMessagesReturned ()
    {
        await ConnectAsync();
        var tasks = new List<Task>();
        for (int i = 0; i < 100; i++)
            if (i % 2 == 0) tasks.Add(MockIncomingAsync<ServerMessageA>());
            else tasks.Add(MockIncomingAsync<ServerMessageB>());
        await Task.WhenAll(tasks);
        Assert.All(tasks, t => Assert.True(t.IsCompletedSuccessfully));
    }

    [Fact]
    public async Task WhenCanceledWhileWaitingMethodReturns ()
    {
        await ConnectAsync();
        using var cts = new CancellationTokenSource(1);
        await Assert.ThrowsAsync<TaskCanceledException>(() => client.WaitAsync<ServerMessage>(cts.Token));
    }

    [Fact]
    public async Task SubscribedHandlerInvoked ()
    {
        var message = default(ServerMessage);
        client.Subscribe<ServerMessage>(m => message = m);
        await ConnectAsync();
        await MockIncomingAsync<ServerMessage>();
        Assert.NotNull(message);
    }

    [Fact, ExcludeFromCodeCoverage]
    public async Task UnsubscribedHandlerNotInvoked ()
    {
        var message = default(ServerMessage);
        client.Subscribe<ServerMessage>(m => message = m);
        client.Unsubscribe<ServerMessage>(m => message = m);
        await ConnectAsync();
        await MockIncomingAsync<ServerMessage>();
        Assert.Null(message);
    }

    private Task<ConnectionStatus> ConnectAsync () => ConnectAsync("");

    private Task<ConnectionStatus> ConnectAsync (string serverName, int port = 0)
    {
        transport.MockIncoming(new ConnectionAccepted { ServerName = serverName });
        return client.ConnectAsync(port);
    }

    private Task<T> MockIncomingAsync<T> () where T : IServerMessage, new()
    {
        var waitTask = client.WaitAsync<T>();
        transport.MockIncoming(new T());
        return waitTask;
    }
}
