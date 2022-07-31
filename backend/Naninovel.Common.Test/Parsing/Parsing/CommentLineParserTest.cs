using Xunit;

namespace Naninovel.Parsing.Test;

public class CommentLineParserTest
{
    private readonly ParseTestHelper<CommentLine> parser = new(new CommentLineParser().Parse);

    [Fact]
    public void ParsesCommentText ()
    {
        Assert.Equal("foo", parser.Parse(";foo").Comment.Text);
        Assert.Empty(parser.Errors);
    }

    [Fact]
    public void CanParseEmptyComment ()
    {
        Assert.Equal("", parser.Parse("; ").Comment.Text);
        Assert.Empty(parser.Errors);
    }

    [Fact]
    public void TrimsCommentText ()
    {
        Assert.Equal("x\tx x", parser.Parse(" ;  x\tx x \t").Comment.Text);
        Assert.Empty(parser.Errors);
    }

    [Fact]
    public void WhenLineIdIsMissingErrorIsAddedAndTextIsEmpty ()
    {
        Assert.Equal("", parser.Parse("foo").Comment.Text);
        Assert.True(parser.HasError(ParsingErrors.MissingLineId));
    }
}
