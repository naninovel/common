using System.Collections.Generic;

namespace Naninovel;

/// <inheritdoc cref="IObserverRegistry{TObserver}"/>
public class ObserverRegistry<TObserver> : IObserverRegistry<TObserver>
{
    private readonly ICollection<TObserver> observers;

    public ObserverRegistry (ICollection<TObserver> observers)
    {
        this.observers = observers;
    }

    public void Register (TObserver observer)
    {
        observers.Add(observer);
    }

    public void Unregister (TObserver observer)
    {
        observers.Remove(observer);
    }
}
