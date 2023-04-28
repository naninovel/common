using System;
using System.Threading;
using Xunit;

namespace Naninovel.Utilities.Test;

public class TextUtilitiesTest
{
    [Fact]
    public void SplitLinesDetectsAllTypesOfLineBreak ()
    {
        var counter = 0;
        foreach (var line in "1\n2\r3\r\n4".SplitLines())
            Assert.Equal((++counter).ToString(), line);
    }

    [Fact]
    public void SplitLinesReturnsSingleElementWhenStringDoesntContainBreaks ()
    {
        Assert.Equal(new[] { "" }, "".SplitLines());
    }

    [Fact]
    public void SplitLinesPreservesEmptyLinesByDefault ()
    {
        Assert.Equal(new[] { "", "" }, "\n".SplitLines());
    }

    [Fact]
    public void SplitLinesRemovesEmptyLinesWhenRequested ()
    {
        Assert.Empty("\n".SplitLines(StringSplitOptions.RemoveEmptyEntries));
    }

    [Fact]
    public void SplitLinesDoesntTrimEntriesByDefault ()
    {
        Assert.Equal(new[] { " ", " " }, " \n ".SplitLines());
    }

    [Fact]
    public void SplitLinesTrimsEntriesWhenRequested ()
    {
        Assert.Equal(new[] { "", "" }, " \n ".SplitLines(StringSplitOptions.TrimEntries));
    }

    [Fact]
    public void IterateLinesDetectsAllTypesOfLineBreak ()
    {
        var counter = 0;
        foreach (var line in "1\n2\r3\r\n4".IterateLines())
            Assert.Equal((++counter).ToString(), line);
    }

    [Fact]
    public void IterateLinesIterateOnceWhenStringDoesntContainBreaks ()
    {
        Assert.Empty("".IterateLines());
    }

    [Fact]
    public void TrimJunkRemovesBomAndZeroWidthSpaceFromStartAndEnd ()
    {
        Assert.Equal("x", "\uFEFF\u200Bx\u200B\u200B".TrimJunk());
    }

    [Fact]
    public void TrimJunkReturnsEmptyWhenStringContainsOnlyJunk ()
    {
        Assert.Empty("\uFEFF\u200B\u200B\u200B\uFEFF\uFEFF\uFEFF".TrimJunk());
    }

    [Fact]
    public void TrimJunkRemovesNothingWhenEmpty ()
    {
        Assert.Empty("".TrimJunk());
    }

    [Fact]
    public void EndWithAndStartWithComparesWithOrdinal ()
    {
        // https://en.wikipedia.org/wiki/Ll
        Thread.CurrentThread.CurrentCulture = new("cy", false);
        Assert.True("ll".StartsWithOrdinal("l"));
        Assert.True("ll".EndsWithOrdinal("l"));
    }

    [Fact]
    public void GetBeforeBehavesCorrectly ()
    {
        Assert.Empty("".GetBefore(""));
        Assert.Empty("".GetBefore(" "));
        Assert.Empty("".GetBefore("x"));
        Assert.Empty("xx".GetBefore("x"));
        Assert.Equal("x", "xw".GetBefore("w"));
        Assert.Equal("x", "xwx".GetBefore("w"));
        Assert.Equal("foo\n", "foo\nbar\nfoo".GetBefore("bar"));
    }

    [Fact]
    public void GetBeforeLastBehavesCorrectly ()
    {
        Assert.Empty("".GetBeforeLast(""));
        Assert.Empty("".GetBeforeLast(" "));
        Assert.Empty("".GetBeforeLast("x"));
        Assert.Equal("x", "xx".GetBeforeLast("x"));
        Assert.Equal("opp", "oppo".GetBeforeLast("o"));
        Assert.Equal("foo\nbar\n", "foo\nbar\nfoo\nnya".GetBeforeLast("foo"));
    }

    [Fact]
    public void GetAfterBehavesCorrectly ()
    {
        Assert.Empty("".GetAfter(""));
        Assert.Empty("".GetAfter(" "));
        Assert.Empty("".GetAfter("x"));
        Assert.Empty("xx".GetAfter("x"));
        Assert.Equal("w", "xw".GetAfter("x"));
        Assert.Empty("xwx".GetAfter("x"));
        Assert.Equal("\n", "foo\nbar\nfoo\n".GetAfter("foo"));
    }

    [Fact]
    public void GetAfterFirstBehavesCorrectly ()
    {
        Assert.Empty("".GetAfterFirst(""));
        Assert.Empty("".GetAfterFirst(" "));
        Assert.Empty("".GetAfterFirst("x"));
        Assert.Equal("x", "xx".GetAfterFirst("x"));
        Assert.Equal("w", "xw".GetAfterFirst("x"));
        Assert.Equal("x", "xwx".GetAfterFirst("w"));
        Assert.Equal("o\nbar\nfoo", "foo\nbar\nfoo".GetAfterFirst("fo"));
    }

    [Fact]
    public void FirstToLowerBehavesCorrectly ()
    {
        Assert.Empty("".FirstToLower());
        Assert.Equal(" ", " ".FirstToLower());
        Assert.Equal("\n\t", "\n\t".FirstToLower());
        Assert.Equal("x", "x".FirstToLower());
        Assert.Equal("xX", "xX".FirstToLower());
        Assert.Equal("x", "X".FirstToLower());
        Assert.Equal("xXX", "XXX".FirstToLower());
    }

    [Fact]
    public void FirstToUpperBehavesCorrectly ()
    {
        Assert.Empty("".FirstToUpper());
        Assert.Equal(" ", " ".FirstToUpper());
        Assert.Equal("\n\t", "\n\t".FirstToUpper());
        Assert.Equal("X", "X".FirstToUpper());
        Assert.Equal("Xx", "Xx".FirstToUpper());
        Assert.Equal("X", "x".FirstToUpper());
        Assert.Equal("XxX", "xxX".FirstToUpper());
    }
}
