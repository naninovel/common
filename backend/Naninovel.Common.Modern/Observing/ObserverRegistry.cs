namespace Naninovel;

/// <inheritdoc cref="IObserverRegistry{TObserver}"/>
public class ObserverRegistry<TObserver> : IObserverRegistry<TObserver>
{
    public IReadOnlyCollection<TObserver> Observers => (IReadOnlyCollection<TObserver>)set;

    private ISet<TObserver> set = new HashSet<TObserver>();

    public void Register (TObserver observer)
    {
        set.Add(observer);
    }

    public void Unregister (TObserver observer)
    {
        set.Remove(observer);
    }

    public void Order (IComparer<TObserver> comparer)
    {
        set = new SortedSet<TObserver>(set, comparer);
    }
}
