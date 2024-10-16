namespace Naninovel;

/// <summary>
/// Extension methods for <see cref="ISerializer"/>.
/// </summary>
public static class SerializerExtensions
{
    /// <inheritdoc cref="ISerializer.Serialize(object)"/>
    public static string? SerializeOrNull (this ISerializer serializer, object? poco) =>
        poco is null ? null : serializer.Serialize(poco);

    /// <inheritdoc cref="ISerializer.Serialize(object,Type)"/>
    public static string? SerializeOrNull (this ISerializer serializer, object? poco, Type type) =>
        poco is null ? null : serializer.Serialize(poco, type);

    /// <inheritdoc cref="ISerializer.Deserialize"/>
    public static object? DeserializeOrNull (this ISerializer serializer, string? serialized, Type type) =>
        string.IsNullOrWhiteSpace(serialized) ? null : serializer.Deserialize(serialized, type);

    /// <inheritdoc cref="Deserialize{T}"/>
    public static T? DeserializeOrNull<T> (this ISerializer serializer, string? serialized) =>
        string.IsNullOrWhiteSpace(serialized) ? default : Deserialize<T>(serializer, serialized);

    /// <summary>
    /// Deserializes specified serialized object string into original object.
    /// </summary>
    /// <param name="serialized">Serialized object string to deserialize.</param>
    /// <typeparam name="T">Type of the original object to deserialize.</typeparam>
    /// <returns>Deserialized object.</returns>
    /// <exception cref="SerializeError">Deserialization failed or produced an incompatible type.</exception>
    public static T Deserialize<T> (this ISerializer serializer, string serialized) =>
        (T)serializer.Deserialize(serialized, typeof(T));

    /// <summary>
    /// Attempts to serialize specified object into string.
    /// </summary>
    /// <param name="poco">Objects to serialize.</param>
    /// <param name="serialized">Serialized string of the object or null when serialization failed.</param>
    /// <returns>Whether serialization was successful.</returns>
    public static bool TrySerialize (this ISerializer serializer, object poco, out string serialized)
    {
        serialized = default!;
        try { serialized = serializer.Serialize(poco); }
        catch { return false; }
        return true;
    }

    /// <summary>
    /// Attempts to serialize specified object into string using serialization context for specified type.
    /// </summary>
    /// <param name="poco">Objects to serialize.</param>
    /// <param name="type">Serialization context of the type.</param>
    /// <param name="serialized">Serialized string of the object or null when serialization failed.</param>
    /// <returns>Whether serialization was successful.</returns>
    public static bool TrySerialize (this ISerializer serializer, object poco, Type type, out string serialized)
    {
        serialized = default!;
        try { serialized = serializer.Serialize(poco, type); }
        catch { return false; }
        return true;
    }

    /// <summary>
    /// Attempts to deserialize specified serialized object string into original object.
    /// </summary>
    /// <param name="serialized">Serialized object string to deserialize.</param>
    /// <param name="poco">Deserialized object or default when deserialization fails.</param>
    /// <typeparam name="T">Type of the original object to deserialize.</typeparam>
    /// <returns>Whether deserialization was successful.</returns>
    public static bool TryDeserialize<T> (this ISerializer serializer, string serialized, out T poco)
    {
        poco = default!;
        try { poco = Deserialize<T>(serializer, serialized); }
        catch { return false; }
        return true;
    }
}
