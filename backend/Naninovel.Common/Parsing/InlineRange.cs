using System;

namespace Naninovel.Parsing;

/// <summary>
/// Represents character range inside a script text line.
/// </summary>
public readonly struct InlineRange : IEquatable<InlineRange>
{
    /// <summary>
    /// First character index of the range.
    /// </summary>
    public readonly int Start;
    /// <summary>
    /// Character count in the range.
    /// </summary>
    public readonly int Length;
    /// <summary>
    /// Last character index of the range.
    /// </summary>
    public int End => Start + Length - 1;

    public InlineRange (int start, int length)
    {
        if (start < 0) throw new ArgumentException("Start index should be greater or equal to zero.", nameof(start));
        if (length < 0) throw new ArgumentException("Length should be greater or equal to zero.", nameof(length));
        Start = start;
        Length = length;
    }

    public override string ToString ()
    {
        return $"{Start}-{End}";
    }

    public bool Equals (InlineRange other)
    {
        return Start == other.Start && Length == other.Length;
    }

    public override bool Equals (object? obj)
    {
        return obj is InlineRange other && Equals(other);
    }

    public override int GetHashCode ()
    {
        unchecked
        {
            return (Start * 397) ^ Length;
        }
    }
}
