namespace Naninovel;

/// <summary>
/// A disposable wrapper over a pooled object.
/// </summary>
/// <remarks>
/// .NET has optimization that prevents boxing disposable structs:
/// https://github.com/dotnet/csharplang/discussions/8337.
/// </remarks>
public readonly struct PooledObject<T> (T obj, ObjectPool<T> pool)
    : IDisposable where T : class
{
    void IDisposable.Dispose () => pool.Return(obj);
}
