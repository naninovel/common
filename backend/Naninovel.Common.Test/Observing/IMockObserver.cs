using System.Threading.Tasks;

namespace Naninovel.Observing.Test;

public interface IMockObserver
{
    void Handle ();
    Task HandleAsync ();
}
