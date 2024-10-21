using System.Threading.Channels;

namespace Naninovel.Bridging.Test;

public class MockTransport : ITransport
{
    public bool Open { get; set; }

    private readonly Channel<string> readChannel = Channel.CreateUnbounded<string>();
    private readonly Channel<string> writeChannel = Channel.CreateUnbounded<string>();
    private readonly CancellationTokenSource cts = new();
    private readonly MessageSerializer serializer = new(new MockSerializer());

    public virtual async Task<string> WaitMessage (CancellationToken token)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, token);
        return await readChannel.Reader.ReadAsync(linkedCts.Token);
    }

    public virtual async Task SendMessage (string message, CancellationToken token)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, token);
        await writeChannel.Writer.WriteAsync(message, linkedCts.Token);
    }

    public void MockIncoming (IMessage message)
    {
        var data = serializer.Serialize(message);
        readChannel.Writer.TryWrite(data);
    }

    public void MockIncoming (string message)
    {
        readChannel.Writer.TryWrite(message);
    }

    public async Task<T> WaitOutcomingAsync<T> () where T : class, IMessage
    {
        while (!cts.IsCancellationRequested)
        {
            var data = await writeChannel.Reader.ReadAsync(cts.Token);
            if (serializer.TryDeserialize<T>(data, out var result)) return result;
        }
        throw new OperationCanceledException();
    }

    public Task Close (CancellationToken token)
    {
        Open = false;
        cts.Cancel();
        return Task.CompletedTask;
    }

    public void Dispose ()
    {
        Open = false;
        cts.Cancel();
        cts.Dispose();
        GC.SuppressFinalize(this);
    }
}
