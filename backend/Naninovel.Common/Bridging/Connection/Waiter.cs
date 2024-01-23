using System.Collections.Concurrent;

namespace Naninovel.Bridging;

internal sealed class Waiter
{
    private readonly object @lock = new();
    private readonly ConcurrentDictionary<Type, List<TaskCompletionSource<IMessage>>> waiters = new();

    public async Task<T> WaitAsync<T> (CancellationToken token) where T : IMessage
    {
        var waiter = new TaskCompletionSource<IMessage>();
        token.Register(waiter.SetCanceled);
        lock (@lock) { GetWaiters(typeof(T)).Add(waiter); }
        return (T)await waiter.Task;
    }

    public void SetResult (IMessage message)
    {
        lock (@lock)
        {
            var type = message.GetType();
            foreach (var waiter in GetWaiters(type))
                waiter.TrySetResult(message);
            GetWaiters(type).Clear();
        }
    }

    private List<TaskCompletionSource<IMessage>> GetWaiters (Type type)
    {
        if (waiters.TryGetValue(type, out var result)) return result;
        return waiters[type] = [];
    }
}
