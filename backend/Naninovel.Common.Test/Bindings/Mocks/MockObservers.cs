using System.Threading.Tasks;
using Naninovel.Observing.Test;

namespace Naninovel.Bindings.Test;

public interface IMockGenericObserver<T> { }

public class MockObserver : IMockObserver
{
    public bool HandleInvoked { get; private set; }
    public bool HandleAsyncInvoked { get; private set; }

    public void Handle () => HandleInvoked = true;

    public Task HandleAsync ()
    {
        HandleAsyncInvoked = true;
        return Task.CompletedTask;
    }
}

public class MockNotifier
{
    private readonly IObserverNotifier<IMockObserver> notifier;

    public MockNotifier (IObserverNotifier<IMockObserver> notifier)
    {
        this.notifier = notifier;
    }

    public void Notify ()
    {
        notifier.Notify(o => o.Handle());
    }

    public async Task NotifyAsync ()
    {
        await notifier.NotifyAsync(o => o.HandleAsync());
    }
}
