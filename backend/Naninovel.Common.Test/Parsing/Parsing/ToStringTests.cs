using Xunit;

namespace Naninovel.Parsing.Test;

public class ToStringTests
{
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
        line.Command.Identifier.Assign("c", 0);
        var param1 = new Parameter();
        param1.Value.Assign("v1", 0);
        line.Command.Parameters.Add(param1);
        var param2 = new Parameter();
        param2.Identifier.Assign("p2", 0);
        param2.Value.Assign("v2", 0);
        line.Command.Parameters.Add(param2);
        Assert.Equal("@c v1 p2:v2", line.ToString());
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
