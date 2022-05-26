using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Naninovel.Bridging;

internal class Waiter
{
    private readonly object @lock = new();
    private readonly IDictionary<Type, List<TaskCompletionSource<IMessage>>> waiters =
        new ConcurrentDictionary<Type, List<TaskCompletionSource<IMessage>>>();

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
        return waiters[type] = new List<TaskCompletionSource<IMessage>>();
    }
}
