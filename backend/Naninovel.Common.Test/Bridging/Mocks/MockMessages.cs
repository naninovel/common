global using static Naninovel.Bridging.Test.MockMessages;

namespace Naninovel.Bridging.Test;

public static class MockMessages
{
    public class ClientMessage : IClientMessage;
    public class ServerMessage : IServerMessage;
    public class ServerMessageA : IServerMessage;
    public class ServerMessageB : IServerMessage;
}
