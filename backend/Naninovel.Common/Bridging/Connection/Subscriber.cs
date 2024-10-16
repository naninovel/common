using System.Collections.Concurrent;

namespace Naninovel.Bridging;

internal class Subscriber
{
    private readonly object @lock = new();
    private readonly IDictionary<Type, HashSet<Subscription>> subscriptions =
        new ConcurrentDictionary<Type, HashSet<Subscription>>();

    public void Subscribe<T> (Action<T> handler) where T : IMessage
    {
        var subscription = new Subscription<T>(handler);
        lock (@lock) { GetSubscriptions(typeof(T)).Add(subscription); }
    }

    public void Unsubscribe<T> (Action<T> handler) where T : IMessage
    {
        var subscription = new Subscription<T>(handler);
        lock (@lock) { GetSubscriptions(typeof(T)).Remove(subscription); }
    }

    public void InvokeHandlers (IMessage message)
    {
        lock (@lock)
        {
            var type = message.GetType();
            foreach (var subscription in GetSubscriptions(type))
                subscription.InvokeHandler(message);
        }
    }

    private HashSet<Subscription> GetSubscriptions (Type type)
    {
        if (subscriptions.TryGetValue(type, out var result)) return result;
        return subscriptions[type] = [];
    }
}
