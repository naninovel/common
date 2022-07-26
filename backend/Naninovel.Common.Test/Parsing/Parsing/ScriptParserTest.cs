using Xunit;

namespace Naninovel.Parsing.Test.Parsing;

public class ScriptParserTest
{
    private readonly ScriptParser parser = new();

    [Fact]
    public void WhenSplitNullReturnsEmptyString ()
    {
        var lines = ScriptParser.SplitText(null);
        Assert.Single(lines);
        Assert.Equal("", lines[0]);
    }

    [Fact]
    public void TextSplitsCorrectly ()
    {
        const string text = " 0 \n\r \r\n3 ";
        var lines = ScriptParser.SplitText(text);
        Assert.Equal(4, lines.Length);
        Assert.Equal(" 0 ", lines[0]);
        Assert.Equal("", lines[1]);
        Assert.Equal(" ", lines[2]);
        Assert.Equal("3 ", lines[3]);
    }
}
