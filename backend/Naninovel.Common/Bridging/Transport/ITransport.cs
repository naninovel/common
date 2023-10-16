namespace Naninovel.Bridging;

public interface ITransport : IDisposable
{
    bool Open { get; }

    Task<string> WaitMessageAsync (CancellationToken token);
    Task SendMessageAsync (string message, CancellationToken token);
    Task CloseAsync (CancellationToken token);
}
