namespace Naninovel.Bridging;

public class ServerInfo
{
    public string Name { get; }
    public int Port { get; }

    internal ServerInfo (string name, int port)
    {
        Name = name;
        Port = port;
    }
}
