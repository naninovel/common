using System;

namespace Naninovel.Parsing;

/// <summary>
/// Represents character range inside a script text line.
/// </summary>
public readonly struct LineRange : IEquatable<LineRange>
{
    /// <summary>
    /// First character index of the range.
    /// </summary>
    public readonly int StartIndex;
    /// <summary>
    /// Character count in the range.
    /// </summary>
    public readonly int Length;
    /// <summary>
    /// Last character index of the range.
    /// </summary>
    public int EndIndex => StartIndex + Length - 1;

    public LineRange (int startIndex, int length)
    {
        if (startIndex < 0) throw new ArgumentException("Start index should be greater or equal to zero.", nameof(startIndex));
        if (length <= 0) throw new ArgumentException("Length should be greater than zero.", nameof(length));
        StartIndex = startIndex;
        Length = length;
    }

    public override string ToString ()
    {
        return $"{StartIndex}-{EndIndex}";
    }

    public bool Equals (LineRange other)
    {
        return StartIndex == other.StartIndex && Length == other.Length;
    }

    public override bool Equals (object obj)
    {
        return obj is LineRange other && Equals(other);
    }

    public override int GetHashCode ()
    {
        unchecked
        {
            return (StartIndex * 397) ^ Length;
        }
    }
}
