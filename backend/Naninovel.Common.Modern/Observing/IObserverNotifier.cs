using System;
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
    void Notify (Action<TObserver> notification);
    /// <summary>
    /// Invokes the notification task on the registered observes.
    /// </summary>
    Task NotifyAsync (Func<TObserver, Task> notification);
}
