using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Naninovel.Bridging.Test;

[ExcludeFromCodeCoverage]
public class MockSerializer : ISerializer
{
    private readonly JsonSerializer json;

    public MockSerializer ()
    {
        var options = new JsonSerializerOptions();
        options.TypeInfoResolverChain.Add(MockContext.Default);
        json = new JsonSerializer(options);
    }

    public string Serialize (object poco) => json.Serialize(poco);
    public string Serialize (object poco, Type type) => json.Serialize(poco, type);
    public object Deserialize (string serialized, Type type) => json.Deserialize(serialized, type);
}
