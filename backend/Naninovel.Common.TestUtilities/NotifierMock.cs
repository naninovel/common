using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;

namespace Naninovel.TestUtilities;

public class NotifierMock<TObserver> : Mock<TObserver>, IObserverNotifier<TObserver>
    where TObserver : class
{
    public override bool CallBase => true;
    public Mock<IObserverNotifier<TObserver>> Mock { get; }

    public NotifierMock ()
    {
        Mock = new Mock<IObserverNotifier<TObserver>>();
    }

    public void Notify (Action<TObserver> notification,
        Func<IEnumerable<TObserver>, IEnumerable<TObserver>> order = null)
    {
        notification(Object);
        Mock.Object.Notify(notification, order);
    }

    public async Task NotifyAsync (Func<TObserver, Task> notification,
        Func<IEnumerable<TObserver>, IEnumerable<TObserver>> order = null)
    {
        await notification(Object);
        await Mock.Object.NotifyAsync(notification, order);
    }
}
