using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Naninovel.Observing.Test;

public class NotifierTest
{
    [Fact]
    public void InvokesNotificationOnEachObserver ()
    {
        var observer = new Mock<IMockObserver>();
        var notifier = new ObserverNotifier<IMockObserver>(new[] { observer.Object });
        notifier.Notify(o => o.Handle());
        observer.Verify(o => o.Handle(), Times.Once);
    }

    [Fact]
    public async Task InvokesNotificationTaskOnEachObserver ()
    {
        var observer = new Mock<IMockObserver>();
        var notifier = new ObserverNotifier<IMockObserver>(new[] { observer.Object });
        await notifier.NotifyAsync(o => o.HandleAsync());
        observer.Verify(o => o.HandleAsync(), Times.Once);
    }
}
