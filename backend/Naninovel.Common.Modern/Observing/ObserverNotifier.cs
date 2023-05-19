using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Naninovel;

/// <inheritdoc cref="IObserverNotifier{TObserver}"/>
public class ObserverNotifier<TObserver> : IObserverNotifier<TObserver>
{
    private readonly IObserverRegistry<TObserver> registry;

    public ObserverNotifier (IObserverRegistry<TObserver> registry)
    {
        this.registry = registry;
    }

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
