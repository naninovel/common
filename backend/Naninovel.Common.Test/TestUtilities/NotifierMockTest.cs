using Moq;

namespace Naninovel.TestUtilities.Test;

public class NotifierMockTest
{
    public interface IMockObserver
    {
        void Handle ();
        Task HandleAsync ();
    }

    private readonly NotifierMock<IMockObserver> notifier = new();

    [Fact]
    public void CallBaseIsEnabled ()
    {
        Assert.True(notifier.CallBase);
    }

    [Fact]
    public void InvokesHandleOnNotify ()
    {
        var handle = new Mock<Action<IMockObserver>>();
        notifier.Notify(handle.Object);
        handle.Verify(h => h.Invoke(It.IsAny<IMockObserver>()), Times.Once);
        notifier.Mock.Verify(n => n.Notify(handle.Object, null), Times.Once);
    }

    [Fact]
    public async Task InvokesHandleAsyncOnNotify ()
    {
        var handleAsync = new Mock<Func<IMockObserver, Task>>();
        await notifier.NotifyAsync(handleAsync.Object);
        handleAsync.Verify(h => h.Invoke(It.IsAny<IMockObserver>()), Times.Once);
        notifier.Mock.Verify(n => n.NotifyAsync(handleAsync.Object, null), Times.Once);
    }
}
