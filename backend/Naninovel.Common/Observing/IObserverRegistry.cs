namespace Naninovel.Observing;

/// <summary>
/// Allows subscribing and unsubscribing observers
/// from receiving notifications by <see cref="IObserverNotifier{TObserver}"/>.
/// </summary>
public interface IObserverRegistry<TObserver>
{
    /// <summary>
    /// Provided observer instance will receive notifications until unsubscribed.
    /// </summary>
    void Register (TObserver observer);
    /// <summary>
    /// Unsubscribes provided observer instance from receiving notifications.
    /// </summary>
    void Unregister (TObserver observer);
}
