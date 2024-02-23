namespace Naninovel.Parsing;

/// <summary>
/// Represents a flag controlling whether command execution should be awaited.
/// </summary>
public class WaitFlag (bool wait) : ILineComponent
{
    /// <summary>
    /// Whether associated command execution should be awaited.
    /// </summary>
    public bool Wait { get; } = wait;

    public static implicit operator bool? (WaitFlag? flag)
    {
        return flag?.Wait;
    }

    public static implicit operator WaitFlag? (bool? wait)
    {
        if (!wait.HasValue) return null;
        return new(wait.Value);
    }

    public override string ToString () =>
        Wait ? Identifiers.WaitTrue : Identifiers.WaitFalse;
}
