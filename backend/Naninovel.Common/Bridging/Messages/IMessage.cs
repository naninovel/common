namespace Naninovel.Bridging;

/// <summary>
/// A bridging service message.
/// </summary>
public interface IMessage { }

/// <summary>
/// A client message to server.
/// </summary>
public interface IClientMessage : IMessage { }

/// <summary>
/// A server message to client.
/// </summary>
public interface IServerMessage : IMessage { }
