using System;

namespace Naninovel.Parsing;

public readonly struct Token : IEquatable<Token>
{
    public readonly TokenType Type;
    public readonly ErrorType Error;
    public readonly int StartIndex;
    public readonly int Length;
    public int EndIndex => StartIndex + Length - 1;

    public Token (TokenType type, int startIndex, int length)
        : this(type, default, startIndex, length) { }

    public Token (ErrorType error, int startIndex, int length)
        : this(TokenType.Error, error, startIndex, length) { }

    private Token (TokenType type, ErrorType error, int startIndex, int length)
    {
        if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex));
        if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));
        Type = type;
        Error = error;
        StartIndex = startIndex;
        Length = length;
    }

    public bool IsError (ErrorType error) => Type == TokenType.Error && Error == error;

    public override string ToString ()
    {
        var type = Type == TokenType.Error ? $"Error#{Error}" : Type.ToString();
        return $"{type} ({StartIndex}-{EndIndex})";
    }

    public override bool Equals (object obj) => obj is Token other && Equals(other);

    public bool Equals (Token other)
    {
        return Type == other.Type &&
               Error == other.Error &&
               StartIndex == other.StartIndex &&
               Length == other.Length;
    }

    public override int GetHashCode ()
    {
        unchecked
        {
            var hashCode = (int)Type;
            hashCode = (hashCode * 397) ^ (int)Error;
            hashCode = (hashCode * 397) ^ StartIndex;
            hashCode = (hashCode * 397) ^ Length;
            return hashCode;
        }
    }
}