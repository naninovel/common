using System.Text.Json.Serialization;
using Naninovel.Bridging;

namespace Naninovel;

[JsonSerializable(typeof(ConnectionAccepted))]
[JsonSerializable(typeof(UpdateMetadata))]
[JsonSerializable(typeof(UpdatePlaybackStatus))]
[JsonSerializable(typeof(GotoRequest))]
[JsonSerializable(typeof(MessageContent))]
internal partial class JsonContext : JsonSerializerContext;
