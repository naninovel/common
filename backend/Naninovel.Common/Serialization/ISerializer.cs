namespace Naninovel;

/// <summary>
/// Implementations is able to de-/serialize arbitrary objects to/from strings.
/// </summary>
public interface ISerializer
{
    /// <summary>
    /// Serializes specified object into string.
    /// </summary>
    /// <param name="poco">Object to serialize.</param>
    /// <returns>Serialized string of the object.</returns>
    /// <exception cref="SerializeError">Serialization failed.</exception>
    string Serialize (object poco);
    /// <summary>
    /// Serializes specified object into string using serializer context for specified type.
    /// </summary>
    /// <remarks>
    /// Use when serializing compiler-generated types, such as collection expressions, in which
    /// case type should be the interface under which the object is assigned, eg IReadOnlyList.
    /// </remarks>
    /// <param name="poco">Object to serialize.</param>
    /// <param name="type">Serialization context of the type.</param>
    /// <returns>Serialized string of the object.</returns>
    /// <exception cref="SerializeError">Serialization failed.</exception>
    string Serialize (object poco, Type type);
    /// <summary>
    /// Deserializes specified string into original object.
    /// </summary>
    /// <param name="serialized">Serialized string of the original object.</param>
    /// <param name="type">Type of the original object.</param>
    /// <returns>Deserialized original object.</returns>
    /// <exception cref="SerializeError">Deserialization failed.</exception>
    object Deserialize (string serialized, Type type);
}
