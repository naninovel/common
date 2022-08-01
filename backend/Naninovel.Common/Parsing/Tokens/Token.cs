using System;

namespace Naninovel.Parsing;

public readonly struct Token : IEquatable<Token>
{
    public readonly TokenType Type;
    public readonly ErrorType Error;
    public readonly LineRange Range;

    public int StartIndex => Range.StartIndex;
    public int EndIndex => Range.EndIndex;
    public int Length => Range.Length;

    public Token (TokenType type, int startIndex, int length)
        : this(type, default, new LineRange(startIndex, length)) { }

    public Token (ErrorType error, int startIndex, int length)
        : this(TokenType.Error, error, new(startIndex, length)) { }

    private Token (TokenType type, ErrorType error, LineRange range)
    {
        Type = type;
        Error = error;
        Range = range;
    }

    public bool IsError (ErrorType error) => Type == TokenType.Error && Error == error;

    public override string ToString ()
    {
        var type = Type == TokenType.Error ? $"Error#{Error}" : Type.ToString();
        return $"{type} ({Range})";
    }

    public bool Equals (Token other)
    {
        return Type == other.Type && Error == other.Error && Range.Equals(other.Range);
    }

    public override bool Equals (object obj)
    {
        return obj is Token other && Equals(other);
    }

    public override int GetHashCode ()
    {
        unchecked
        {
            var hashCode = (int)Type;
            hashCode = (hashCode * 397) ^ (int)Error;
            hashCode = (hashCode * 397) ^ Range.GetHashCode();
            return hashCode;
        }
    }
}
