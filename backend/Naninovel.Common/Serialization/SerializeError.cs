namespace Naninovel;

/// <summary>
/// Exception thrown from Naninovel internal behaviour when serialization fails.
/// </summary>
public class SerializeError (string message) : Error(message);
