using System;
using System.Threading;
using System.Threading.Tasks;

namespace Naninovel.Bridging;

public class Connection : IDisposable
{
    private readonly ITransport transport;
    private readonly Subscriber subscriber;
    private readonly Waiter waiter;
    private readonly SendQueue sendQueue = new();
    private readonly CancellationTokenSource cts = new();

    internal Connection (ITransport transport,
        Subscriber subscriber = default, Waiter waiter = default)
    {
        this.transport = transport;
        this.subscriber = subscriber ?? new Subscriber();
        this.waiter = waiter ?? new Waiter();
    }

    internal async Task MaintainAsync ()
    {
        try { await await Task.WhenAny(RunSendLoopAsync(), RunReceiveLoopAsync()); }
        catch (OperationCanceledException) { }
    }

    public void Send (IMessage message)
    {
        sendQueue.Enqueue(message);
    }

    public void Subscribe<T> (Action<T> handler) where T : IMessage
    {
        subscriber.Subscribe(handler);
    }

    public void Unsubscribe<T> (Action<T> handler) where T : IMessage
    {
        subscriber.Unsubscribe(handler);
    }

    public Task<T> WaitAsync<T> (CancellationToken token = default) where T : IMessage
    {
        var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(token, cts.Token);
        return waiter.WaitAsync<T>(combinedCts.Token);
    }

    public async Task CloseAsync (CancellationToken token = default)
    {
        await transport.CloseAsync(token);
        cts.Cancel();
    }

    public void Dispose ()
    {
        cts.Dispose();
        transport.Dispose();
        sendQueue.Dispose();
    }

    private async Task RunSendLoopAsync ()
    {
        while (transport.Open && !cts.IsCancellationRequested)
        {
            var message = await sendQueue.WaitAsync(cts.Token);
            var data = Serializer.Serialize(message);
            await transport.SendMessageAsync(data, cts.Token);
        }
    }

    private async Task RunReceiveLoopAsync ()
    {
        while (transport.Open && !cts.IsCancellationRequested)
        {
            var data = await transport.WaitMessageAsync(cts.Token);
            if (!Serializer.TryDeserialize(data, out var message)) continue;
            subscriber.InvokeHandlers(message);
            waiter.SetResult(message);
        }
    }
}
