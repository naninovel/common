namespace Naninovel.Bridging;

/// <summary>
/// Sent by the server when client connection request is accepted.
/// </summary>
public class ConnectionAccepted : IServerMessage
{
    /// <summary>
    /// Name of the server that accepted the connection.
    /// </summary>
    public string ServerName { get; set; } = "";
}
