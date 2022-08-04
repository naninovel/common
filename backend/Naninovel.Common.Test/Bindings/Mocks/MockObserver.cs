using System.Threading.Tasks;
using Naninovel.Observing.Test;

namespace Naninovel.Bindings.Test;

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
