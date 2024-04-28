namespace Naninovel.Expression;

/// <summary>
/// Access to a variable stored in a map.
/// </summary>
internal class Map (string name, IExpression key) : IExpression
{
    /// <summary>
    /// Identifier of the map.
    /// </summary>
    public string Name { get; } = name;
    /// <summary>
    /// Key of the accessed value.
    /// </summary>
    public IExpression Key { get; } = key;
}
