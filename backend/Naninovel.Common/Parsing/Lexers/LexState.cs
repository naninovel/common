using System;
using System.Collections.Generic;

namespace Naninovel.Parsing;

public class LexState
{
    public int Index { get; private set; }
    public int Length => text.Length;
    public int EndIndex => text.Length - 1;
    public bool EndReached => Index > EndIndex;
    public bool IsSpace => IsIndexValid(Index) && char.IsWhiteSpace(text[Index]);
    public bool IsNotSpace => IsIndexValid(Index) && !char.IsWhiteSpace(text[Index]);
    public bool IsPreviousSpace => IsIndexValid(Index - 1) && char.IsWhiteSpace(text[Index - 1]);

    private string text = "";
    private ICollection<Token> tokens = Array.Empty<Token>();

    public void Reset (string text, ICollection<Token> tokens)
    {
        this.text = text ?? throw new ArgumentNullException(nameof(text));
        this.tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        if (tokens.IsReadOnly) throw new ArgumentException("Collection shouldn't be read-only.", nameof(tokens));
        Index = 0;
    }

    public int Move () => ++Index;

    public void AddToken (TokenType type, int startIndex, int length)
    {
        var token = new Token(type, startIndex, length);
        tokens.Add(token);
    }

    public void AddError (ErrorType type, int startIndex, int length)
    {
        var token = new Token(type, startIndex, length);
        tokens.Add(token);
    }

    public void SkipSpace ()
    {
        while (IsSpace) Index++;
    }

    public bool Is (char @char)
    {
        return IsIndexValid(Index) && text[Index] == @char;
    }

    public bool IsUnescaped (char @char)
    {
        return Is(@char) && !Utilities.IsEscaped(text, Index);
    }

    private bool IsIndexValid (int index)
    {
        return text.Length > 0 && index >= 0 && index < text.Length;
    }
}
