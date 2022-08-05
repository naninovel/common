using System.Threading.Tasks;
using Naninovel.Observing.Test;

namespace Naninovel.Bindings.Test;

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
