using System.Runtime.CompilerServices;

namespace Naninovel;

/// <summary>
/// Controls behaviour of the <see cref="ObjectPool{T}"/> instances.
/// </summary>
public abstract class ObjectPool
{
    /// <summary>
    /// Whether the pool is active, ie reuses pooled objects and invokes specified
    /// rent, return and drop hooks on the pooled objects.
    /// </summary>
    /// <remarks>
    /// Use to globally disable the pooling behaviour for edge cases, such as unit tests,
    /// where MOQ verification is not possible when the objects are pooled.
    /// </remarks>
    public static bool PoolingEnabled { get; set; } = true;
}

/// <summary>
/// Allows re-using object instances to limit heap allocations.
/// </summary>
/// <typeparam name="T">Type of the pooled objects.</typeparam>
public sealed class ObjectPool<T> : ObjectPool, IDisposable where T : class
{
    /// <summary>
    /// Configures the pool behaviour.
    /// </summary>
    public sealed class Options
    {
        /// <summary>
        /// The initial size of the pool.
        /// </summary>
        public int MinSize { get; set; } = 10;
        /// <summary>
        /// The pool size limit, at which point it'll overflow and ignore
        /// returned objects, allowing them to be garbage-collected.
        /// </summary>
        public int MaxSize { get; set; } = 10000;
        /// <summary>
        /// Callback to invoke on the rented objects.
        /// </summary>
        public Action<T>? OnRent { get; set; }
        /// <summary>
        /// Callback to invoke on the returned objects.
        /// </summary>
        public Action<T>? OnReturn { get; set; }
        /// <summary>
        /// Callback to invoke on the dropped objects: ignored on return due
        /// to overflow or objects dropped on <see cref="ObjectPool{T}.Drop"/>.
        /// </summary>
        public Action<T>? OnDrop { get; set; }
    }

    /// <summary>
    /// The total number of objects managed by the pool.
    /// </summary>
    public int Total { get; private set; }
    /// <summary>
    /// Number of rented objects, ie rented and not returned.
    /// </summary>
    public int Rented => Total - Available;
    /// <summary>
    /// Number of "free" objects, ie rented and returned, available for rent w/o allocation.
    /// </summary>
    public int Available => pool.Count + (lastReturned != null ? 1 : 0);

    private readonly List<T> pool;
    private readonly Func<T> factory;
    private readonly Options options;
    private T? lastReturned;

    /// <param name="factory">Factory function to create pooled objects.</param>
    /// <param name="options">Options to configure the pool behaviour.</param>
    public ObjectPool (Func<T> factory, Options? options = null)
    {
        this.factory = factory;
        this.options = options ?? new();
        pool = new List<T>(this.options.MinSize);
    }

    /// <summary>
    /// Rents an object from the pool.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Rent ()
    {
        if (!PoolingEnabled) return factory();

        T obj;
        if (lastReturned != null)
        {
            obj = lastReturned;
            lastReturned = null;
        }
        else if (pool.Count == 0)
        {
            obj = factory();
            Total++;
        }
        else
        {
            var idx = pool.Count - 1;
            obj = pool[idx];
            pool.RemoveAt(idx);
        }
        options.OnRent?.Invoke(obj);
        return obj;
    }

    /// <summary>
    /// Rents an object from the pool and creates a disposable wrapper for auto-return.
    /// </summary>
    /// <param name="obj">The rented object.</param>
    /// <returns>Disposable wrapper over the rented object, which will return the object on dispose.</returns>
    public PooledObject<T> Rent (out T obj) => new(obj = Rent(), this);

    /// <summary>
    /// Returns specified previously rented object back to the pool,
    /// so that it can be re-used later without allocations.
    /// </summary>
    /// <param name="obj">Object to return.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Return (T obj)
    {
        if (!PoolingEnabled) return;

        options.OnReturn?.Invoke(obj);
        if (lastReturned == null)
        {
            lastReturned = obj;
        }
        else if (Available < options.MaxSize)
        {
            pool.Add(obj);
        }
        else
        {
            Total--;
            options.OnDrop?.Invoke(obj);
        }
    }

    /// <summary>
    /// Drops the pooled objects, allowing them to be reclaimed by the garbage collector.
    /// </summary>
    public void Drop ()
    {
        if (options.OnDrop is { } drop)
        {
            foreach (var obj in pool)
                drop(obj);
            if (lastReturned != null)
                drop(lastReturned);
        }

        Total = 0;
        pool.Clear();
        lastReturned = null;
    }

    public void Dispose () => Drop();
}
