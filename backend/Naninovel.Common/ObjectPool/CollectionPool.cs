namespace Naninovel;

/// <summary>
/// Base class for collection pools.
/// </summary>
/// <typeparam name="TCollection">Type of the pooled collection.</typeparam>
/// <typeparam name="TItem">Type of the pooled collection items.</typeparam>
public class CollectionPool<TCollection, TItem> where TCollection : class, ICollection<TItem>, new()
{
    internal static readonly ObjectPool<TCollection> pool = new(() => [], new() { OnReturn = c => c.Clear() });

    /// <inheritdoc cref="ObjectPool{T}.Rent()"/>
    public static TCollection Rent () => pool.Rent();
    /// <inheritdoc cref="ObjectPool{T}.Rent(out T)"/>
    public static PooledObject<TCollection> Rent (out TCollection obj) => pool.Rent(out obj);
    /// <inheritdoc cref="ObjectPool{T}.Return"/>
    public static void Return (TCollection obj) => pool.Return(obj);
    /// <inheritdoc cref="ObjectPool{T}.Drop"/>
    public static void Drop () => pool.Drop();
}

/// <summary>
/// Collection pool over <see cref="List{T}"/> objects.
/// </summary>
/// <typeparam name="T">Type of the pooled list items.</typeparam>
public sealed class ListPool<T> : CollectionPool<List<T>, T>;

/// <summary>
/// Collection pool over <see cref="HashSet{T}"/> objects.
/// </summary>
/// <typeparam name="T">Type of the pooled set items.</typeparam>
public sealed class SetPool<T> : CollectionPool<HashSet<T>, T>;

/// <summary>
/// Collection pool over <see cref="Dictionary{TKey,TValue}"/> objects.
/// </summary>
/// <typeparam name="TKey">Type of the pooled map keys.</typeparam>
/// <typeparam name="TValue">Type of the pooled map values.</typeparam>
public sealed class MapPool<TKey, TValue> : CollectionPool<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>;
