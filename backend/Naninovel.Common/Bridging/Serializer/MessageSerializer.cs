namespace Naninovel.Bridging;

public class MessageSerializer
{
    private readonly ISerializer serializer;

    public MessageSerializer (ISerializer serializer)
    {
        this.serializer = serializer;
    }

    public string Serialize (IMessage message)
    {
        var content = new MessageContent {
            MessageType = message.GetType().AssemblyQualifiedName,
            SerializedMessage = serializer.Serialize(message)
        };
        return serializer.Serialize(content);
    }

    public bool TryDeserialize (string serializedMessage, out IMessage message)
    {
        try
        {
            var content = serializer.Deserialize<MessageContent>(serializedMessage);
            var type = Type.GetType(content.MessageType);
            message = (IMessage)serializer.Deserialize(content.SerializedMessage, type);
            return true;
        }
        catch
        {
            message = null!;
            return false;
        }
    }
}
