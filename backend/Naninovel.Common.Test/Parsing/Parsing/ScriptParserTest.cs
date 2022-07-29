using System.Collections.Generic;
using Xunit;

namespace Naninovel.Parsing.Test;

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

    [Fact]
    public void ScriptTextParsed ()
    {
        const string scriptText = @"
; Comment line
@commandLine
Generic text line
# LabelLine
";
        var lines = parser.ParseText(scriptText);
        Assert.IsType<GenericLine>(lines[0]);
        Assert.IsType<CommentLine>(lines[1]);
        Assert.IsType<CommandLine>(lines[2]);
        Assert.IsType<GenericLine>(lines[3]);
        Assert.IsType<LabelLine>(lines[4]);
    }

    [Fact]
    public void LexingErrorPreserved ()
    {
        var errors = new List<ParseError>();
        parser.ParseLine("# x  x", errors);
        Assert.Single(errors);
        Assert.Equal(LexingErrors.GetFor(ErrorType.SpaceInLabel), errors[0].Message);
        Assert.Equal(3, errors[0].StartIndex);
        Assert.Equal(2, errors[0].Length);
        Assert.Equal(4, errors[0].EndIndex);
    }
}
