using System.Text.Json;

namespace Naninovel;

public class JsonSerializer : ISerializer
{
    private readonly JsonSerializerOptions options;

    public JsonSerializer (JsonSerializerOptions? options = null)
    {
        this.options = options ?? new();
        // TODO: Remove after Unity supports non-field JSON serialization.
        this.options.IncludeFields = true;
        this.options.TypeInfoResolverChain.Add(JsonContext.Default);
    }

    public string Serialize (object poco)
    {
        var type = poco.GetType();
        if (!options.TryGetTypeInfo(type, out var info))
            throw new SerializeError($"Failed to serialize '{type.FullName}' object: missing type info.");
        return System.Text.Json.JsonSerializer.Serialize(poco, info);
    }

    public object Deserialize (string serialized, Type type)
    {
        if (!options.TryGetTypeInfo(type, out var info))
            throw new SerializeError($"Failed to deserialize '{type.FullName}' object: missing type info.");
        return System.Text.Json.JsonSerializer.Deserialize(serialized, info) ??
               throw new SerializeError($"Failed to deserialize '{serialized}' JSON to '{type.FullName}' object: result is null.");
    }
}
