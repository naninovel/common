namespace Naninovel.Utilities.Test;

public class TextUtilsTest
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
    public void IterateLinesIndexedDetectsAllTypesOfLineBreak ()
    {
        foreach (var (line, index) in "0\n1\r2\r\n3".IterateLinesIndexed())
            Assert.Equal((index).ToString(), line);
    }

    [Fact]
    public void IterateLinesIndexedIterateOnceWhenStringDoesntContainBreaks ()
    {
        Assert.Empty("".IterateLinesIndexed());
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
        Assert.Equal("", "/foo".GetBefore("bar"));
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
        Assert.Equal("", "/foo".GetBeforeLast("bar"));
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
        Assert.Equal("", "/foo".GetAfter("bar"));
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
        Assert.Equal("", "/foo".GetAfterFirst("bar"));
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

    [Theory]
    [InlineData("", new[] { "" })]
    [InlineData("a", new[] { "a" })]
    [InlineData("a,b", new[] { "a", "b" })]
    [InlineData("abc,a,abc,b,abc,c", new[] { "abc", "a", "abc", "b", "abc", "c" })]
    [InlineData(" a , b , c ", new[] { " a ", " b ", " c " })]
    [InlineData("a,,c", new[] { "a", "", "c" })]
    [InlineData(",", new[] { "", "" })]
    [InlineData(",,", new[] { "", "", "" })]
    [InlineData(",,c", new[] { "", "", "c" })]
    [InlineData(@"a\,b", new[] { "a,b" })]
    [InlineData("a\n,b\n\\,", new[] { "a\n", "b\n," })]
    [InlineData(@"\,", new[] { "," })]
    [InlineData(@"\,\,,", new[] { ",,", "" })]
    [InlineData(@"\,,\,,\,", new[] { ",", ",", "," })]
    [InlineData(@"\\,\\,\,", new[] { @"\\", @"\\", "," })]
    public void SplitAndJoinEscapedWorksCorrectly (string input, string[] expected)
    {
        var splitter = new TextSplitter(',');
        var entries = new List<string>();
        splitter.Split(input, entries);
        Assert.Equal(expected, entries);
        Assert.Equal(input, splitter.Join(entries));
    }

    [Theory]
    [InlineData("", new char[] { }, "")]
    [InlineData("", new[] { ' ' }, "")]
    [InlineData(" ", new[] { ' ' }, "")]
    [InlineData(" ", new char[] { }, " ")]
    [InlineData("xyz", new[] { 'x', 'y' }, "z")]
    [InlineData("xxx-yyy-zzz", new[] { 'x', 'x', 'y', '-' }, "zzz")]
    [InlineData("xxx-yyy-zzz", new[] { 'a', 'b', 'c' }, "xxx-yyy-zzz")]
    public void SanitizeWorksCorrectly (string input, IReadOnlyCollection<char> invalid, string expected)
    {
        Assert.Equal(expected, input.Sanitize(invalid));
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(" ", "")]
    [InlineData("_-\\+^@", "")]
    [InlineData("FooBar", "Foo Bar")]
    [InlineData("fooBar", "Foo Bar")]
    [InlineData("foo_bar", "Foo Bar")]
    [InlineData("FOO_BAR", "Foo Bar")]
    [InlineData("FOO BAR", "Foo Bar")]
    [InlineData("foo-bar", "Foo Bar")]
    [InlineData("Foo Bar", "Foo Bar")]
    [InlineData("Foo1", "Foo 1")]
    [InlineData("Foo 1", "Foo 1")]
    [InlineData("Foo123Bar321Nya", "Foo 123 Bar 321 Nya")]
    [InlineData("Foo 123Bar", "Foo 123 Bar")]
    [InlineData(" Foo 123 Bar ", "Foo 123 Bar")]
    [InlineData("Foo-123_Bar", "Foo 123 Bar")]
    [InlineData("_foo_", "Foo")]
    [InlineData("__foo__", "Foo")]
    [InlineData("___foo___", "Foo")]
    [InlineData(" _ __ FOO1-2-3BAR __ _ ", "Foo 1 2 3 Bar")]
    public void HumanizeWorksCorrectly (string input, string expected)
    {
        Assert.Equal(expected, input.Humanize());
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(" ", "")]
    [InlineData("_-\\+^@", "")]
    [InlineData("FooBar", "FooBar")]
    [InlineData("fooBar", "FooBar")]
    [InlineData("foo_bar", "FooBar")]
    [InlineData("FOO_BAR", "FooBar")]
    [InlineData("FOO BAR", "FooBar")]
    [InlineData("foo-bar", "FooBar")]
    [InlineData("Foo Bar", "FooBar")]
    [InlineData("Foo1", "Foo1")]
    [InlineData("Foo 1", "Foo1")]
    [InlineData("Foo 123 Bar 321 Nya", "Foo123Bar321Nya")]
    [InlineData("Foo 123Bar", "Foo123Bar")]
    [InlineData(" Foo 123 Bar ", "Foo123Bar")]
    [InlineData("Foo-123_Bar", "Foo123Bar")]
    [InlineData("_foo_", "Foo")]
    [InlineData("__foo__", "Foo")]
    [InlineData("___foo___", "Foo")]
    [InlineData(" _ __ FOO1-2-3BAR __ _ ", "Foo123Bar")]
    public void ToPascalCaseWorksCorrectly (string input, string expected)
    {
        Assert.Equal(expected, input.ToPascalCase());
    }
}
