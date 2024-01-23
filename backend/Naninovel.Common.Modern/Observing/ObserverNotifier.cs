namespace Naninovel;

/// <inheritdoc cref="IObserverNotifier{TObserver}"/>
public class ObserverNotifier<TObserver>(IObserverRegistry<TObserver> registry) : IObserverNotifier<TObserver>
{
    public void Notify (Action<TObserver> notification,
        Func<IEnumerable<TObserver>, IEnumerable<TObserver>>? order = null)
    {
        foreach (var observer in order?.Invoke(registry.Observers) ?? registry.Observers)
            notification(observer);
    }

    public async Task NotifyAsync (Func<TObserver, Task> notification,
        Func<IEnumerable<TObserver>, IEnumerable<TObserver>>? order = null)
    {
        foreach (var observer in order?.Invoke(registry.Observers) ?? registry.Observers)
            await notification(observer);
    }
}
