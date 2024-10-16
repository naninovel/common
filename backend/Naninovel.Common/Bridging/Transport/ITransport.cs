namespace Naninovel.Bridging;

public interface ITransport : IDisposable
{
    bool Open { get; }

    Task<string> WaitMessage (CancellationToken token);
    Task SendMessage (string message, CancellationToken token);
    Task Close (CancellationToken token);
}
