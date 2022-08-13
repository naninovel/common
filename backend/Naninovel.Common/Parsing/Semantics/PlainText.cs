using System.Collections;
using System.Collections.Generic;

namespace Naninovel.Parsing;

/// <summary>
/// Represents plain text content.
/// </summary>
public class PlainText : ILineComponent, IValueComponent, IEnumerable<char>
{
    public static readonly PlainText Empty = new(string.Empty);

    /// <summary>
    /// The underlying text content.
    /// </summary>
    public string Text { get; }

    public PlainText (string text)
    {
        Text = text;
    }

    public static implicit operator string (PlainText? plainText)
    {
        return plainText?.Text!;
    }

    public static implicit operator PlainText (string? @string)
    {
        if (@string is null) return null!;
        if (@string == string.Empty) return Empty;
        return new(@string);
    }

    public char this [int index] => Text[index];

    public IEnumerator<char> GetEnumerator ()
    {
        return Text.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
        return GetEnumerator();
    }

    public override string ToString () => Text;
}
