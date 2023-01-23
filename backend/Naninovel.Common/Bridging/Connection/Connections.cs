using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Naninovel.Bridging;

internal class Connections
{
    private readonly IDictionary<Connection, byte> connections =
        new ConcurrentDictionary<Connection, byte>();

    public int Count => connections.Count;

    public void Add (Connection connection)
    {
        connections[connection] = default;
    }

    public void Remove (Connection connection)
    {
        connections.Remove(connection);
    }

    public void Clear ()
    {
        connections.Clear();
    }

    public IEnumerable<Connection> Enumerate ()
    {
        return connections.Keys;
    }
}
