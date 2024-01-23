using System.Text.Json;

namespace Naninovel.Bridging.Test;

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
    public object Deserialize (string serialized, Type type) => json.Deserialize(serialized, type);
}
