using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Naninovel.Bridging.Test;

public class MockTransport : ITransport
{
    public bool Open { get; set; }

    private readonly Channel<string> readChannel = Channel.CreateUnbounded<string>();
    private readonly Channel<string> writeChannel = Channel.CreateUnbounded<string>();
    private readonly CancellationTokenSource cts = new();

    public virtual async Task<string> WaitMessageAsync (CancellationToken token)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, token);
        return await readChannel.Reader.ReadAsync(linkedCts.Token);
    }

    public virtual async Task SendMessageAsync (string message, CancellationToken token)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, token);
        await writeChannel.Writer.WriteAsync(message, linkedCts.Token);
    }

    public void MockIncoming (IMessage message)
    {
        var data = Serializer.Serialize(message);
        readChannel.Writer.TryWrite(data);
    }

    public void MockIncoming (string message)
    {
        readChannel.Writer.TryWrite(message);
    }

    [ExcludeFromCodeCoverage]
    public async Task<T> WaitOutcomingAsync<T> () where T : IMessage
    {
        while (!cts.IsCancellationRequested)
        {
            var data = await writeChannel.Reader.ReadAsync(cts.Token);
            if (Serializer.TryDeserialize(data, out var m) && m is T result) return result;
        }
        throw new OperationCanceledException();
    }

    public Task CloseAsync (CancellationToken token)
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
