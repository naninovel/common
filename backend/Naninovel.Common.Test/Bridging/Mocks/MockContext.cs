using System.Text.Json.Serialization;

namespace Naninovel.Bridging.Test;

[JsonSerializable(typeof(ClientMessage))]
[JsonSerializable(typeof(ServerMessage))]
[JsonSerializable(typeof(ServerMessageA))]
[JsonSerializable(typeof(ServerMessageB))]
internal partial class MockContext : JsonSerializerContext;
