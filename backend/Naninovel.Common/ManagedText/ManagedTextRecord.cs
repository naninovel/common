﻿namespace Naninovel.ManagedText;

/// <summary>
/// A key-value pair with an optional comment contained in <see cref="ManagedTextDocument"/>.
/// </summary>
/// <remarks>
/// Hashed (identified) by <see cref="Key"/> only.
/// </remarks>
public readonly struct ManagedTextRecord (string key, string? value = null, string? comment = null)
    : IEquatable<ManagedTextRecord>
{
    /// <summary>
    /// Unique (inside document) identifier of the record.
    /// </summary>
    public readonly string Key = key;
    /// <summary>
    /// Value of the record or empty.
    /// </summary>
    public readonly string Value = value ?? string.Empty;
    /// <summary>
    /// Optional remark associated with the record or empty.
    /// </summary>
    public readonly string Comment = comment ?? string.Empty;

    public override string ToString ()
    {
        return $"{Key}->{Value}{(string.IsNullOrEmpty(Comment) ? "" : $" ({Comment})")}";
    }

    public bool Equals (ManagedTextRecord other)
    {
        return Key == other.Key;
    }

    public override bool Equals (object? obj)
    {
        return obj is ManagedTextRecord other && Equals(other);
    }

    public override int GetHashCode ()
    {
        return Key != null ? Key.GetHashCode() : 0;
    }
}
