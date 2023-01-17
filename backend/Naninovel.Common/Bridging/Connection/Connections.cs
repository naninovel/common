using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Naninovel.Bridging;

internal class Connections : IEnumerable<Connection>
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

    public IEnumerator<Connection> GetEnumerator ()
    {
        return connections.Keys.GetEnumerator();
    }

    [ExcludeFromCodeCoverage]
    IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
}
