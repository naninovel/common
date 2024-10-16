namespace Naninovel;

/// <summary>
/// A common <see cref="ObjectPool{T}"/> singleton.
/// </summary>
/// <typeparam name="T">Type of the pooled objects.</typeparam>
public static class GenericPool<T> where T : class, new()
{
    private static readonly ObjectPool<T> pool = new(() => new T());

    /// <inheritdoc cref="ObjectPool{T}.Rent()"/>
    public static T Rent () => pool.Rent();
    /// <inheritdoc cref="ObjectPool{T}.Rent(out T)"/>
    public static PooledObject<T> Rent (out T obj) => pool.Rent(out obj);
    /// <inheritdoc cref="ObjectPool{T}.Return"/>
    public static void Return (T obj) => pool.Return(obj);
    /// <inheritdoc cref="ObjectPool{T}.Drop"/>
    public static void Drop () => pool.Drop();
}
