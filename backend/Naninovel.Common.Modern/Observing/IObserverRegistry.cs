namespace Naninovel;

/// <summary>
/// Allows subscribing and unsubscribing observers
/// from receiving notifications by <see cref="IObserverNotifier{TObserver}"/>.
/// </summary>
public interface IObserverRegistry<TObserver>
{
    /// <summary>
    /// Registered observers either in registration or custom order set via <see cref="Order"/>.
    /// </summary>
    IReadOnlyCollection<TObserver> Observers { get; }

    /// <summary>
    /// Provided observer instance will receive notifications until unsubscribed.
    /// </summary>
    void Register (TObserver observer);
    /// <summary>
    /// Unsubscribes provided observer instance from receiving notifications.
    /// </summary>
    void Unregister (TObserver observer);
    /// <summary>
    /// Re-orders existing and keeps future observers in the specified order;
    /// by default the observers are ordered and notified in registration order.
    /// </summary>
    void Order (IComparer<TObserver> comparer);
}
