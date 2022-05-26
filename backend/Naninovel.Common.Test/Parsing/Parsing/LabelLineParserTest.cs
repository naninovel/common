using Xunit;
using static Naninovel.Parsing.ErrorType;

namespace Naninovel.Parsing.Test;

public class LabelLineParserTest : LineParserTest<LabelLineParser, LabelLine>
{
    protected override string ExampleLine => "# Label";

    [Fact]
    public void LabelLineParsed ()
    {
        var line = Parse(ExampleLine);
        Assert.Equal("Label", line.LabelText);
    }

    [Fact]
    public void WhenSpaceErrorIsAdded ()
    {
        Parse("# Label With Spaces");
        Assert.True(HasError(SpaceInLabel));
    }

    [Fact]
    public void WhenSpaceLabelIsTrimmed ()
    {
        var line = Parse("# Label With Spaces");
        Assert.Equal("Label", line.LabelText);
    }

    [Fact]
    public void WhenMissingErrorIsAdded ()
    {
        Parse("# ");
        Assert.True(HasError(MissingLabel));
    }

    [Fact]
    public void WhenMissingLabelIsEmpty ()
    {
        var line = Parse("# ");
        Assert.Equal(string.Empty, line.LabelText);
    }

    [Fact]
    public void IndexesEvaluatedCorrectly ()
    {
        var line = Parse("# x");
        Assert.Equal(0, line.StartIndex);
        Assert.Equal(3, line.Length);
        Assert.Equal(2, line.LabelText.StartIndex);
        Assert.Equal(1, line.LabelText.Length);
    }
}
