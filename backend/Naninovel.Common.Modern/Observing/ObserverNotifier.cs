using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Naninovel;

/// <inheritdoc cref="IObserverNotifier{TObserver}"/>
public class ObserverNotifier<TObserver> : IObserverNotifier<TObserver>
{
    private readonly IReadOnlyCollection<TObserver> observers;

    public ObserverNotifier (IReadOnlyCollection<TObserver> observers)
    {
        this.observers = observers;
    }

    public void Notify (Action<TObserver> notification,
        Func<IEnumerable<TObserver>, IEnumerable<TObserver>>? order = null)
    {
        foreach (var observer in order?.Invoke(observers) ?? observers)
            notification(observer);
    }

    public async Task NotifyAsync (Func<TObserver, Task> notification,
        Func<IEnumerable<TObserver>, IEnumerable<TObserver>>? order = null)
    {
        foreach (var observer in order?.Invoke(observers) ?? observers)
            await notification(observer);
    }
}
