namespace Naninovel.Parsing;

public class LineText : LineContent
{
    public string Text { get; set; } = string.Empty;

    public static implicit operator string (LineText text) => text?.Text;

    public void Assign (string text, int startIndex)
    {
        Text = text;
        StartIndex = startIndex;
        Length = text.Length;
    }

    public override string ToString () => Text;
}
