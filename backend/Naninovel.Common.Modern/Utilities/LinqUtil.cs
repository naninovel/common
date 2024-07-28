namespace Naninovel;

/// <summary>
/// Common LINQ-related helpers and extensions.
/// </summary>
public static class LinqUtil
{
    /// <summary>
    /// Checks whether specified index is in bounds for the collection.
    /// </summary>
    public static bool IsIndexValid<T> (this IEnumerable<T> e, int index)
    {
        if (index < 0) return false;
        if (e is IReadOnlyCollection<T> c) return index < c.Count;
        if (e is string str) return index < str.Length;
        var count = 0;
        foreach (var _ in e) count++;
        return index < count;
    }

    /// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>
    /// <remarks>This overload for structs returns null instead of default value.</remarks>
    public static T? FirstOrNull<T> (this IEnumerable<T> e) where T : struct
    {
        return e.AtOrNull(0);
    }

    /// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource, bool})"/>
    /// <remarks>This overload for structs returns null instead of default value.</remarks>
    public static T? FirstOrNull<T> (this IEnumerable<T> e, Func<T, bool> predicate) where T : struct
    {
        foreach (var element in e)
            if (predicate(element))
                return element;
        return null;
    }

    /// <inheritdoc cref="Enumerable.LastOrDefault{TSource}(System.Collections.Generic.IEnumerable{TSource})"/>
    /// <remarks>This overload for structs returns null instead of default value.</remarks>
    public static T? LastOrNull<T> (this IEnumerable<T> e) where T : struct
    {
        if (e is IReadOnlyList<T> list)
        {
            var count = list.Count;
            if (count > 0) return list[count - 1];
            return null;
        }
        var last = default(T?);
        foreach (var element in e)
            last = element;
        return last;
    }

    /// <inheritdoc cref="Enumerable.LastOrDefault{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource, bool})"/>
    /// <remarks>This overload for structs returns null instead of default value.</remarks>
    public static T? LastOrNull<T> (this IEnumerable<T> e, Func<T, bool> predicate) where T : struct
    {
        if (e is IReadOnlyList<T> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
                if (predicate(list[i]))
                    return list[i];
            return null;
        }
        var last = default(T?);
        foreach (var element in e)
            if (predicate(element))
                last = element;
        return last;
    }

    /// <inheritdoc cref="Enumerable.ElementAtOrDefault{TSource}(System.Collections.Generic.IEnumerable{TSource},System.Index)"/>
    /// <remarks>This overload for structs returns null instead of default value.</remarks>
    public static T? AtOrNull<T> (this IEnumerable<T> e, int index) where T : struct
    {
        if (e is IReadOnlyList<T> list) return list.IsIndexValid(index) ? list[index] : null;
        var i = 0;
        foreach (var element in e)
            if (i++ == index)
                return element;
        return null;
    }
}
