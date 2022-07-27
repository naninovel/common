using Xunit;

namespace Naninovel.Parsing.Test;

public class CommentLineParserTest
{
    private readonly ParseTestHelper<CommentLine> parser = new(new CommentLineParser().Parse);

    [Fact]
    public void ParsesCommentText ()
    {
        Assert.Equal("foo", parser.Parse(";foo").Comment);
        Assert.Empty(parser.Errors);
    }

    [Fact]
    public void CanParseEmptyComment ()
    {
        Assert.Equal("", parser.Parse("; ").Comment);
        Assert.Empty(parser.Errors);
    }

    [Fact]
    public void TrimsCommentText ()
    {
        Assert.Equal("x\tx x", parser.Parse(" ;  x\tx x \t").Comment);
        Assert.Empty(parser.Errors);
    }

    [Fact]
    public void WhenLineIdIsMissingErrorIsAddedAndTextIsEmpty ()
    {
        Assert.Equal("", parser.Parse("foo").Comment);
        Assert.True(parser.HasError(ParsingErrors.MissingLineId));
    }
}
