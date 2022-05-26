using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Naninovel.Bridging.Test;

public sealed class MockServerTransport : IServerTransport
{
    public bool Listening { get; set; }
    public int Port { get; private set; }
    public bool ThrowOnConnection { get; set; }

    private readonly Channel<MockTransport> queue = Channel.CreateUnbounded<MockTransport>();
    private CancellationTokenSource cts = new();

    public void StartListening (int port)
    {
        cts = new CancellationTokenSource();
        Listening = true;
        Port = port;
    }

    public void StopListening ()
    {
        cts.Cancel();
        Listening = false;
    }

    public async Task<ITransport> WaitConnectionAsync (CancellationToken token)
    {
        using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(token, cts.Token);
        return await queue.Reader.ReadAsync(combinedCts.Token);
    }

    public void MockIncomingConnection (MockTransport transport)
    {
        queue.Writer.TryWrite(transport);
    }

    public void Dispose ()
    {
        cts.Cancel();
        cts.Dispose();
    }
}
