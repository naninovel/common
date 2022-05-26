using System;
using System.Threading;
using System.Threading.Tasks;

namespace Naninovel.Bridging;

public interface IServerTransport : IDisposable
{
    bool Listening { get; }

    void StartListening (int port);
    void StopListening ();
    Task<ITransport> WaitConnectionAsync (CancellationToken token);
}
