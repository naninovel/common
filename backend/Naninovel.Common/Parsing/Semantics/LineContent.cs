namespace Naninovel.Parsing;

public abstract class LineContent
{
    public int StartIndex { get; set; }
    public int Length { get; set; }
    public int EndIndex => StartIndex + Length - 1;
    public bool Empty => Length <= 0;
}
