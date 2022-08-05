using System.Threading.Tasks;
using Naninovel.Observing.Test;

namespace Naninovel.Bindings.Test;

public interface IMockGenericObserver<T> { }

public interface IMockNotifier
{
    void Notify ();
    Task NotifyAsync ();
}

public interface IMockObserverService
{
    bool HandleInvoked { get; }
    bool HandleAsyncInvoked { get; }
}

public class MockObserver : IMockObserverService, IMockObserver
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

public class MockNotifier : IMockNotifier
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
