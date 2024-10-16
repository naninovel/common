using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Naninovel.Bridging;

internal class SendQueue : IDisposable
{
    private readonly ConcurrentQueue<IMessage> queue = new();
    private readonly SemaphoreSlim semaphore = new(0);

    public void Enqueue (IMessage message)
    {
        queue.Enqueue(message);
        semaphore.Release();
    }

    [ExcludeFromCodeCoverage]
    public async Task<IMessage> Wait (CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await semaphore.WaitAsync(token);
            if (queue.TryDequeue(out var message)) return message;
        }
        throw new OperationCanceledException();
    }

    public void Dispose () => semaphore.Dispose();
}
