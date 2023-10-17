using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Naninovel.Metadata;

namespace Naninovel.Bridging.Test;

public class SerializerTest
{
    [ExcludeFromCodeCoverage] public record Record(string Value);
    [ExcludeFromCodeCoverage] public record Message(Record[] Records);

    [Fact]
    public void CanSerializeCommonTypes ()
    {
        var serializer = new JsonSerializer();
        var update = new UpdateMetadata { Metadata = new Project { Resources = new[] { new Resource { Type = "foo", Path = "bar" } } } };
        Assert.True(serializer.TrySerialize(update, out var serialized));
        Assert.True(serializer.TryDeserialize<UpdateMetadata>(serialized, out var deserialized));
        Assert.Equal("foo", deserialized.Metadata.Resources[0].Type);
        Assert.Equal("bar", deserialized.Metadata.Resources[0].Path);
    }

    [Fact]
    public void CanSerializeAdditionalTypes ()
    {
        var options = new JsonSerializerOptions();
        options.TypeInfoResolverChain.Add(AdditionalContext.Default);
        var serializer = new JsonSerializer(options);
        var message = new Message(new[] { new Record("foo") });
        Assert.True(serializer.TrySerialize(message, out var serialized));
        Assert.True(serializer.TryDeserialize<Message>(serialized, out var deserialized));
        Assert.Equal("foo", deserialized.Records[0].Value);
    }

    [Fact]
    public void ThrowsWhenMissingTypeInfo ()
    {
        var serializer = new JsonSerializer();
        Assert.Contains("missing type info", Assert.Throws<SerializeError>(() => serializer.Serialize(new Record("foo"))).Message);
        Assert.Contains("missing type info", Assert.Throws<SerializeError>(() => serializer.Deserialize<Record>("")).Message);
    }

    [Fact]
    public void ThrowsWhenResultIsNull ()
    {
        var serializer = new JsonSerializer();
        Assert.Contains("result is null", Assert.Throws<SerializeError>(() => serializer.Deserialize<Resource>("null")).Message);
    }

    [Fact]
    public void FailsWhenSerializingNull ()
    {
        Assert.False(new JsonSerializer().TrySerialize(null, out _));
    }

    [Fact]
    public void FailsWhenDeserializingMalformedJson ()
    {
        Assert.False(new JsonSerializer().TryDeserialize<Resource>("{", out _));
    }
}

[JsonSerializable(typeof(SerializerTest.Message))]
internal partial class AdditionalContext : JsonSerializerContext;
