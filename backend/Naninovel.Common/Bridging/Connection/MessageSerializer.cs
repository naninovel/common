namespace Naninovel.Bridging;

public class MessageSerializer
{
    public class Content
    {
        public string MessageType { get; set; } = "";
        public string SerializedMessage { get; set; } = "";
    }

    private readonly ISerializer serializer;

    public MessageSerializer (ISerializer serializer)
    {
        this.serializer = serializer;
    }

    public string Serialize (IMessage message)
    {
        var content = new Content {
            MessageType = message.GetType().AssemblyQualifiedName,
            SerializedMessage = serializer.Serialize(message)
        };
        return serializer.Serialize(content);
    }

    public bool TryDeserialize (string serializedMessage, out IMessage message)
    {
        try
        {
            var content = serializer.Deserialize<Content>(serializedMessage);
            var type = Type.GetType(content.MessageType);
            message = (IMessage)serializer.Deserialize(content.SerializedMessage, type);
            return true;
        }
        catch (InvalidOperationException)
        {
            message = null!;
            return false;
        }
    }

    public bool TryDeserialize<T> (string serializedMessage, out T message)
        where T : class, IMessage
    {
        TryDeserialize(serializedMessage, out var baseMessage);
        return (message = (baseMessage as T)!) != null;
    }
}
