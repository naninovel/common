using Xunit;

namespace Naninovel.Parsing.Test;

public class CommentLineParserTest : LineParserTest<CommentLineParser, CommentLine>
{
    protected override string ExampleLine => "; Comment Text";

    [Fact]
    public void CommentLineParsed ()
    {
        var line = Parse(ExampleLine);
        Assert.Equal("Comment Text", line.CommentText);
    }

    [Fact]
    public void WhenMissingTextIsEmpty ()
    {
        var line = Parse("; ");
        Assert.Equal(string.Empty, line.CommentText);
    }
        
    [Fact]
    public void IndexesEvaluatedCorrectly ()
    {
        var line = Parse("; x");
        Assert.Equal(0, line.StartIndex);
        Assert.Equal(3, line.Length);
        Assert.Equal(2, line.CommentText.StartIndex);
        Assert.Equal(1, line.CommentText.Length);
    }
}
