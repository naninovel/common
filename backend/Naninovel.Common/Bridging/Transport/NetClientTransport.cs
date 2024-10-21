using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Text;

namespace Naninovel.Bridging;

[ExcludeFromCodeCoverage]
public class NetClientTransport : IClientTransport
{
    public bool Open => socket.State == WebSocketState.Open;

    private const int bufferSize = 1024;
    private readonly byte[] receiveBuffer = new byte[bufferSize];
    private readonly byte[] sendBuffer = new byte[bufferSize];
    private readonly ClientWebSocket socket = new();

    public Task ConnectToServer (int port, CancellationToken token)
    {
        return socket.ConnectAsync(new Uri($"ws://localhost:{port}"), token);
    }

    public async Task<string> WaitMessage (CancellationToken token)
    {
        using var stream = new MemoryStream();
        var segment = new ArraySegment<byte>(receiveBuffer);
        var result = default(WebSocketReceiveResult);
        do
        {
            result = await socket.ReceiveAsync(segment, token);
            if (result.MessageType == WebSocketMessageType.Close)
                throw new OperationCanceledException();
            stream.Write(segment.Array ?? [], segment.Offset, result.Count);
        } while (!result.EndOfMessage);
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    public async Task SendMessage (string message, CancellationToken token)
    {
        var messageLength = message.Length;
        var messageCount = (int)Math.Ceiling((double)messageLength / bufferSize);
        for (var i = 0; i < messageCount; i++)
        {
            var offset = bufferSize * i;
            var count = bufferSize;
            var lastMessage = i + 1 == messageCount;
            if (count * (i + 1) > messageLength)
                count = messageLength - offset;
            var segmentLength = Encoding.UTF8.GetBytes(message, offset, count, sendBuffer, 0);
            var segment = new ArraySegment<byte>(sendBuffer, 0, segmentLength);
            await socket.SendAsync(segment, WebSocketMessageType.Text, lastMessage, token);
        }
    }

    public Task Close (CancellationToken token)
    {
        return socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", token);
    }

    public void Dispose () => socket.Dispose();
}
