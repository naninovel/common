﻿using static Naninovel.Parsing.ErrorType;
using static Naninovel.Parsing.ParsingErrors;

namespace Naninovel.Parsing.Test;

public class LabelLineParserTest
{
    private readonly ParseTestHelper<LabelLine> parser = new(h => new LabelLineParser(h).Parse);

    [Fact]
    public void ParsesLabelText ()
    {
        Assert.Equal("foo", parser.Parse("#foo").Label.Text);
        Assert.Empty(parser.Errors);
    }

    [Fact]
    public void WhenLabelMissingErrorIsAddedAndLabelIsEmpty ()
    {
        Assert.Equal("", parser.Parse("# ").Label.Text);
        Assert.True(parser.HasError(MissingLabel));
    }

    [Fact]
    public void TrimsLabelText ()
    {
        Assert.Equal("foo", parser.Parse(" #  foo \t").Label.Text);
        Assert.Empty(parser.Errors);
    }

    [Fact]
    public void WhenLineIdIsMissingErrorIsAddedAndLabelIsEmpty ()
    {
        Assert.Equal("", parser.Parse("foo").Label.Text);
        Assert.True(parser.HasError(MissingLineId));
    }

    [Fact]
    public void WhenSpaceInLabelErrorIsAddedAndLabelIsEmpty ()
    {
        Assert.Equal("", parser.Parse("# foo bar").Label.Text);
        Assert.True(parser.HasError(SpaceInLabel));
    }

    [Fact]
    public void RangesAreAssociatedCorrectly ()
    {
        var line = parser.Parse("# label");
        Assert.Equal(new(2, 5), parser.Resolve(line.Label));
    }
}
