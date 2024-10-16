namespace Naninovel.Bridging;

public class Connection : IDisposable
{
    private readonly ITransport transport;
    private readonly MessageSerializer serializer;
    private readonly Subscriber subscriber;
    private readonly Waiter waiter;
    private readonly SendQueue sendQueue = new();
    private readonly CancellationTokenSource cts = new();

    internal Connection (ITransport transport, ISerializer serializer,
        Subscriber? subscriber = default, Waiter? waiter = default)
    {
        this.transport = transport;
        this.serializer = new MessageSerializer(serializer);
        this.subscriber = subscriber ?? new Subscriber();
        this.waiter = waiter ?? new Waiter();
    }

    internal async Task Maintain ()
    {
        try { await await Task.WhenAny(RunSendLoop(), RunReceiveLoop()); }
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

    public async Task Close (CancellationToken token = default)
    {
        await transport.Close(token);
        cts.Cancel();
    }

    public void Dispose ()
    {
        cts.Dispose();
        transport.Dispose();
        sendQueue.Dispose();
    }

    private async Task RunSendLoop ()
    {
        while (transport.Open && !cts.IsCancellationRequested)
        {
            var message = await sendQueue.Wait(cts.Token);
            var data = serializer.Serialize(message);
            await transport.SendMessage(data, cts.Token);
        }
    }

    private async Task RunReceiveLoop ()
    {
        while (transport.Open && !cts.IsCancellationRequested)
        {
            var data = await transport.WaitMessage(cts.Token);
            if (!serializer.TryDeserialize(data, out var message)) continue;
            subscriber.InvokeHandlers(message);
            waiter.SetResult(message);
        }
    }
}
