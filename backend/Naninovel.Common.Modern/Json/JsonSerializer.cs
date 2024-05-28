using System.Text.Json;

namespace Naninovel;

public class JsonSerializer : ISerializer
{
    public JsonSerializerOptions Options { get; }

    public JsonSerializer (JsonSerializerOptions? options = null)
    {
        Options = options ?? new();
        Options.TypeInfoResolverChain.Add(JsonContext.Default);
    }

    public string Serialize (object poco)
    {
        var type = poco.GetType();
        return Serialize(poco, type);
    }

    public string Serialize (object poco, Type type)
    {
        if (!Options.TryGetTypeInfo(type, out var info))
            throw new SerializeError($"Failed to serialize '{type.FullName}' object: missing type info.");
        return System.Text.Json.JsonSerializer.Serialize(poco, info);
    }

    public object Deserialize (string serialized, Type type)
    {
        if (!Options.TryGetTypeInfo(type, out var info))
            throw new SerializeError($"Failed to deserialize '{type.FullName}' object: missing type info.");
        return System.Text.Json.JsonSerializer.Deserialize(serialized, info) ??
               throw new SerializeError($"Failed to deserialize '{serialized}' JSON to '{type.FullName}' object: result is null.");
    }
}
