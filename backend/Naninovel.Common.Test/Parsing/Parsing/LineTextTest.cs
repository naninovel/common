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
}
