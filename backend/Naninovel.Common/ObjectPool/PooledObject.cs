namespace Naninovel;

/// <summary>
/// A disposable wrapper over a pooled object.
/// </summary>
public readonly struct PooledObject<T> (T obj, ObjectPool<T> pool)
    : IDisposable where T : class
{
    void IDisposable.Dispose () => pool.Return(obj);
}
