using System;

namespace Naninovel.Metadata;

/// <summary>
/// Represents navigation position inside scenario script.
/// </summary>
public readonly struct Endpoint : IEquatable<Endpoint>
{
    /// <summary>
    /// Name of the script; when null represents current script.
    /// </summary>
    public string? Script { get; }
    /// <summary>
    /// Label inside script; when null represents start of the script.
    /// </summary>
    public string? Label { get; }

    public Endpoint (string? script, string? label)
    {
        Script = script;
        Label = label;
    }

    public bool Equals (Endpoint other)
    {
        return Script == other.Script && Label == other.Label;
    }

    public override bool Equals (object? obj)
    {
        return obj is Endpoint other && Equals(other);
    }

    public override int GetHashCode ()
    {
        unchecked { return ((Script != null ? Script.GetHashCode() : 0) * 397) ^ (Label != null ? Label.GetHashCode() : 0); }
    }
}
