namespace Naninovel.Expression;

internal class ParseContext
{
    public int Index { get; private set; }
    public int Length => text.Length;
    public int EndIndex => text.Length - 1;
    public char Char => text[Index];
    public bool EndReached => Index > EndIndex;
    public bool IsSpace => IsIndexValid(Index) && char.IsWhiteSpace(text[Index]);
    public bool IsNotSpace => IsIndexValid(Index) && !char.IsWhiteSpace(text[Index]);
    public bool IsPreviousSpace => char.IsWhiteSpace(text.ElementAtOrDefault(Index - 1));
    public bool IsNextSpace => char.IsWhiteSpace(text.ElementAtOrDefault(Index + 1));
    public bool IsQuoted { get; private set; }
    public int Level { get; private set; }
    public bool IsTopAndUnquoted => Level == 0 && !IsQuoted;

    private string text = "";

    public void Reset (string text)
    {
        this.text = text;
        Index = 0;
        Level = 0;
        IsQuoted = false;
    }

    public int Move ()
    {
        if (IsUnescaped('"')) IsQuoted = !IsQuoted;
        else if (!IsQuoted && IsUnescaped('(')) Level++;
        else if (!IsQuoted && IsUnescaped(')')) Level--;
        return ++Index;
    }

    public char Consume () => text[Move() - 1];

    public bool Is (char @char)
    {
        return IsIndexValid(Index) && text[Index] == @char;
    }

    public bool Is (Predicate<char> predicate)
    {
        return IsIndexValid(Index) && predicate(text[Index]);
    }

    public bool IsPrevious (char @char)
    {
        return IsIndexValid(Index - 1) && text[Index - 1] == @char;
    }

    public bool IsNext (char @char)
    {
        return IsIndexValid(Index + 1) && text[Index + 1] == @char;
    }

    private bool IsIndexValid (int index)
    {
        return text.Length > 0 && index >= 0 && index < text.Length;
    }

    public bool IsUnescaped (char @char)
    {
        return Is(@char) && !IsPrevious('\\');
    }
}
