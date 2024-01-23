namespace Naninovel.Bridging;

public class ServerInfo (string name, int port)
{
    public string Name { get; } = name;
    public int Port { get; } = port;
}
