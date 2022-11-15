using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Naninovel;

/// <summary>
/// Allows notifying observers registered via <see cref="IObserverRegistry{TObserver}"/>.
/// </summary>
public interface IObserverNotifier<TObserver>
{
    /// <summary>
    /// Invokes the notification action on the registered observes.
    /// </summary>
    /// <param name="notification">The action to invoke on each observer.</param>
    /// <param name="order">Allows changing notification order; by default observers are notified in the register order.</param>
    void Notify (Action<TObserver> notification, Func<IEnumerable<TObserver>, IEnumerable<TObserver>>? order = null);
    /// <summary>
    /// Invokes the notification task on the registered observes.
    /// </summary>
    /// <inheritdoc cref="Notify"/>
    Task NotifyAsync (Func<TObserver, Task> notification, Func<IEnumerable<TObserver>, IEnumerable<TObserver>>? order = null);
}
