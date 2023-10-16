namespace Naninovel.Bridging.Test;

public class MockClientTransport : MockTransport, IClientTransport
{
    public List<MockServerTransport> MockServers { get; } = new();

    private class ReverseTransport : MockTransport
    {
        private readonly MockClientTransport client;

        public ReverseTransport (MockClientTransport client)
        {
            this.client = client;
            Open = true;
        }

        public override Task SendMessageAsync (string message, CancellationToken token)
        {
            client.MockIncoming(message);
            return Task.CompletedTask;
        }
    }

    public Task ConnectToServerAsync (int port, CancellationToken token)
    {
        ThrowIfRequested(port);
        MockAcceptanceDelayed(port);
        Open = true;
        return Task.CompletedTask;
    }

    private async void MockAcceptanceDelayed (int port)
    {
        await Task.Yield();
        var server = MockServers.FirstOrDefault(s => s.Port == port);
        server?.MockIncomingConnection(new ReverseTransport(this));
    }

    private void ThrowIfRequested (int port)
    {
        var server = MockServers.FirstOrDefault(s => s.Port == port);
        if (server is { ThrowOnConnection: true })
            throw new Exception("Throw requested by mock");
    }
}
