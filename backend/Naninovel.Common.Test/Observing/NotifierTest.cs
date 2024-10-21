using Moq;

namespace Naninovel.Observing.Test;

public class NotifierTest
{
    [Fact]
    public void InvokesNotificationOnEachObserver ()
    {
        var observer = new Mock<IMockObserver>();
        var registry = new Mock<IObserverRegistry<IMockObserver>>();
        registry.SetupGet(r => r.Observers).Returns([observer.Object]);
        var notifier = new ObserverNotifier<IMockObserver>(registry.Object);
        notifier.Notify(o => o.Handle());
        observer.Verify(o => o.Handle(), Times.Once);
    }

    [Fact]
    public async Task InvokesNotificationTaskOnEachObserver ()
    {
        var observer = new Mock<IMockObserver>();
        var registry = new Mock<IObserverRegistry<IMockObserver>>();
        registry.SetupGet(r => r.Observers).Returns([observer.Object]);
        var notifier = new ObserverNotifier<IMockObserver>(registry.Object);
        await notifier.NotifyAsync(o => o.HandleAsync());
        observer.Verify(o => o.HandleAsync(), Times.Once);
    }

    [Fact]
    public void ByDefaultNotifiesInOrder ()
    {
        var notifyCounter = 0;
        var observer1 = new Mock<IMockObserver>();
        var observer2 = new Mock<IMockObserver>();
        var registry = new Mock<IObserverRegistry<IMockObserver>>();
        registry.SetupGet(r => r.Observers).Returns([observer1.Object, observer2.Object]);
        observer1.Setup(o => o.Handle()).Callback(() => Assert.Equal(1, ++notifyCounter));
        observer2.Setup(o => o.Handle()).Callback(() => Assert.Equal(2, ++notifyCounter));
        var notifier = new ObserverNotifier<IMockObserver>(registry.Object);
        notifier.Notify(o => o.Handle());
    }

    [Fact]
    public void CanChangeNotifyOrder ()
    {
        var notifyCounter = 0;
        var observer1 = new Mock<IMockObserver>();
        var observer2 = new Mock<IMockObserver>();
        var registry = new Mock<IObserverRegistry<IMockObserver>>();
        registry.SetupGet(r => r.Observers).Returns([observer1.Object, observer2.Object]);
        observer1.Setup(o => o.Handle()).Callback(() => Assert.Equal(2, ++notifyCounter));
        observer2.Setup(o => o.Handle()).Callback(() => Assert.Equal(1, ++notifyCounter));
        var notifier = new ObserverNotifier<IMockObserver>(registry.Object);
        notifier.Notify(o => o.Handle(), o => o.Reverse());
    }

    [Fact]
    public async Task CanChangeNotifyOrderAsync ()
    {
        var notifyCounter = 0;
        var observer1 = new Mock<IMockObserver>();
        var observer2 = new Mock<IMockObserver>();
        observer1.Setup(o => o.HandleAsync()).Callback(() => Assert.Equal(2, ++notifyCounter));
        observer2.Setup(o => o.HandleAsync()).Callback(() => Assert.Equal(1, ++notifyCounter));
        var registry = new Mock<IObserverRegistry<IMockObserver>>();
        registry.SetupGet(r => r.Observers).Returns([observer1.Object, observer2.Object]);
        var notifier = new ObserverNotifier<IMockObserver>(registry.Object);
        await notifier.NotifyAsync(o => o.HandleAsync(), o => o.Reverse());
    }
}
