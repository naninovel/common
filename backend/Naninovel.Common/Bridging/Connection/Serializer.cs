using System;
using System.IO;
using System.Xml.Serialization;

namespace Naninovel.Bridging;

public static class Serializer
{
    public class Content
    {
        public string MessageType { get; set; } = "";
        public string SerializedMessage { get; set; } = "";
    }

    public static string Serialize (IMessage message)
    {
        var content = new Content {
            MessageType = message.GetType().AssemblyQualifiedName,
            SerializedMessage = SerializeXml(message)
        };
        return SerializeXml(content);
    }

    public static bool TryDeserialize (string serializedMessage, out IMessage message)
    {
        try
        {
            var content = (Content)DeserializeXml(serializedMessage, typeof(Content));
            var type = Type.GetType(content.MessageType);
            message = (IMessage)DeserializeXml(content.SerializedMessage, type);
            return true;
        }
        catch (InvalidOperationException)
        {
            message = null!;
            return false;
        }
    }

    public static bool TryDeserialize<T> (string serializedMessage, out T message)
        where T : class, IMessage
    {
        TryDeserialize(serializedMessage, out var baseMessage);
        return (message = (baseMessage as T)!) != null;
    }

    private static string SerializeXml (object data)
    {
        var serializer = new XmlSerializer(data.GetType());
        using var writer = new StringWriter();
        serializer.Serialize(writer, data);
        return writer.ToString();
    }

    private static object DeserializeXml (string xml, Type dataType)
    {
        var serializer = new XmlSerializer(dataType);
        using var reader = new StringReader(xml);
        return serializer.Deserialize(reader);
    }
}
