namespace Naninovel.Bridging;

public interface IClientTransport : ITransport
{
    Task ConnectToServerAsync (int port, CancellationToken token);
}
