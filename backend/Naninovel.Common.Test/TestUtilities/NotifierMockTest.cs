using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;

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
        Func<IEnumerable<IMockObserver>, IEnumerable<IMockObserver>> order = o => o;
        notifier.Notify(handle.Object, order);
        handle.Verify(h => h.Invoke(It.IsAny<IMockObserver>()), Times.Once);
        notifier.Mock.Verify(n => n.Notify(handle.Object, order), Times.Once);
    }

    [Fact]
    public async Task InvokesHandleAsyncOnNotify ()
    {
        var handleAsync = new Mock<Func<IMockObserver, Task>>();
        Func<IEnumerable<IMockObserver>, IEnumerable<IMockObserver>> order = o => o;
        await notifier.NotifyAsync(handleAsync.Object, order);
        handleAsync.Verify(h => h.Invoke(It.IsAny<IMockObserver>()), Times.Once);
        notifier.Mock.Verify(n => n.NotifyAsync(handleAsync.Object, order), Times.Once);
    }
}
