using System.Threading;
using System.Threading.Tasks;

namespace Naninovel.Bridging;

public interface IClientTransport : ITransport
{
    Task ConnectToServerAsync (int port, CancellationToken token);
}
