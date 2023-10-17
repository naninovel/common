namespace Naninovel.Bridging;

[Serializable]
public class MessageContent
{
    public string MessageType { get; set; } = string.Empty;
    public string SerializedMessage { get; set; } = string.Empty;
}
