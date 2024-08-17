namespace Naninovel.Bridging;

public interface IClientTransport : ITransport
{
    Task ConnectToServer (int port, CancellationToken token);
}
