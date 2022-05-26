using Xunit;

namespace Naninovel.Parsing.Test;

public class LineTextTest
{
    [Fact]
    public void EndIndexEvaluatedCorrectly ()
    {
        Assert.Equal(0, new LineText { StartIndex = 0, Length = 1 }.EndIndex);
    }

    [Fact]
    public void EmptyEvaluatedCorrectly ()
    {
        var emptyContent = new LineText();
        Assert.True(emptyContent.Empty);
        var notEmptyContent = new LineText();
        notEmptyContent.Assign("x", 0);
        Assert.False(notEmptyContent.Empty);
    }

    [Fact]
    public void CommentLineToStringIsCorrect ()
    {
        var comment = new CommentLine();
        comment.CommentText.Assign("x", 0);
        Assert.Equal("; x", comment.ToString());
    }

    [Fact]
    public void LabelLineToStringIsCorrect ()
    {
        var line = new LabelLine();
        line.LabelText.Assign("x", 0);
        Assert.Equal("# x", line.ToString());
    }

    [Fact]
    public void CommandLineToStringIsCorrect ()
    {
        var line = new CommandLine();
        line.Command.Identifier.Assign("i", 0);
        var parameter = new Parameter();
        parameter.Identifier.Assign("p", 0);
        parameter.Value.Assign("v", 0);
        line.Command.Parameters.Add(parameter);
        Assert.Equal("@i p:v", line.ToString());
    }

    [Fact]
    public void GenericLineToStringIsCorrect ()
    {
        var line = new GenericTextLine();
        var text = new GenericText();
        text.Assign("x", 0);
        line.Content.Add(text);
        Assert.Equal("x", line.ToString());
    }

    [Fact]
    public void GenericLineWithPrefixAndContentToStringIsCorrect ()
    {
        var line = new GenericTextLine();
        line.Prefix.AuthorIdentifier.Assign("a", 0);
        line.Prefix.AuthorAppearance.Assign("b", 2);
        line.Prefix.StartIndex = 0;
        line.Prefix.Length = 5;
        var text = new GenericText();
        text.Assign("x", 6);
        line.Content.Add(text);
        var inlined = new InlinedCommand();
        inlined.Command.Identifier.Assign("i", 8);
        line.Content.Add(inlined);
        Assert.Equal("a.b: x[i]", line.ToString());
    }
}
