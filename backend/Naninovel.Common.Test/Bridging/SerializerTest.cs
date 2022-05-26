using Xunit;

namespace Naninovel.Bridging.Test;

public class SerializerTest
{
    public class Record
    {
        public string Value { get; set; }
    }

    public class Message : IMessage
    {
        public Record[] Records { get; set; }
    }

    [Fact]
    public void MessageSurvivesSerialization ()
    {
        var message = new Message { Records = new[] { new Record { Value = "Foo" } } };
        var serialized = Serializer.Serialize(message);
        Assert.True(Serializer.TryDeserialize<Message>(serialized, out var deserialized));
        Assert.Equal("Foo", deserialized.Records[0].Value);
    }
}
