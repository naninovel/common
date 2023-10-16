namespace Naninovel;

/// <summary>
/// Exception thrown from Naninovel internal behaviour when serialization fails.
/// </summary>
public class SerializeError : Error
{
    public SerializeError (string message) : base(message) { }
}
