namespace Naninovel.Metadata;

/// <summary>
/// Represents navigation position inside scenario script.
/// </summary>
public readonly struct Endpoint (string? scriptPath, string? label)
    : IEquatable<Endpoint>
{
    /// <summary>
    /// Resource path of the script; when null represents current script.
    /// </summary>
    public string? ScriptPath { get; } = scriptPath;
    /// <summary>
    /// Label inside script; when null represents beginning of the script.
    /// </summary>
    public string? Label { get; } = label;

    public bool Equals (Endpoint other)
    {
        return ScriptPath == other.ScriptPath && Label == other.Label;
    }

    public override bool Equals (object? obj)
    {
        return obj is Endpoint other && Equals(other);
    }

    public override int GetHashCode ()
    {
        unchecked { return ((ScriptPath != null ? ScriptPath.GetHashCode() : 0) * 397) ^ (Label != null ? Label.GetHashCode() : 0); }
    }
}
