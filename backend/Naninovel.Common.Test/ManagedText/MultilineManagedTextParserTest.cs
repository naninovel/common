using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Naninovel.ManagedText.Test;

public class MultilineManagedTextParserTest
{
    public static IEnumerable<object[]> Facts { get; } = new[] {
        Fact("", Array.Empty<ManagedTextRecord>()),
        Fact("\n\r \n\t", Array.Empty<ManagedTextRecord>()),
        Fact(";", Array.Empty<ManagedTextRecord>()),
        Fact("key: value", Array.Empty<ManagedTextRecord>()),
        Fact("; comment\nkey: value", Array.Empty<ManagedTextRecord>()),
        Fact(" # k", Array.Empty<ManagedTextRecord>()),
        Fact("# key", new ManagedTextRecord("key")),
        Fact("#key\n", new ManagedTextRecord("key")),
        Fact("# key \t\n", new ManagedTextRecord("key")),
        Fact("# k \t ey \t\n", new ManagedTextRecord("k \t ey")),
        Fact("#key1\n#key2\n", new("key1", ""), new("key2", "")),
        Fact("# key\n; comment\n", new ManagedTextRecord("key", "", "comment")),
        Fact("# key\n; comment\nvalue\n", new ManagedTextRecord("key", "value", "comment")),
        Fact("# key\nvalue\n; comment\n", new ManagedTextRecord("key", "value", "comment")),
        Fact("# key1\nvalue\n; comment\n# key2\n", new("key1", "value", "comment"), new("key2", "", "")),
        Fact("# keyA\na 1 \na2\n# keyB\n b 1\nb2 \n", new("keyA", "a 1 a2"), new("keyB", " b 1b2 ")),
        Fact("# key\nfoo\n\nbar\n", new ManagedTextRecord("key", "foobar")),
        Fact("#key1\n\n\nfoo\nbar\n\n\n#key2\n\n\nvalue\n\n\n", new("key1", "foobar"), new("key2", "value"))
    };

    private readonly MultilineManagedTextParser parser = new();

    [Theory, MemberData(nameof(Facts))]
    public void ParseTheory (string text, params ManagedTextRecord[] expected)
    {
        var records = parser.Parse(text).Records.ToArray();
        Assert.Equal(expected.Length, records.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i].Key, records[i].Key);
            Assert.Equal(expected[i].Value, records[i].Value);
            Assert.Equal(expected[i].Comment, records[i].Comment);
        }
    }

    [Fact]
    public void CommentOnFirstLineIsParsedAsHeader ()
    {
        Assert.Equal("foo", parser.Parse(";foo").Header);
    }

    [Fact]
    public void HeaderIsTrimmed ()
    {
        Assert.Equal("foo", parser.Parse("; \tfoo \t").Header);
    }

    [Fact]
    public void WhenCommentOnFirstLineIsWhitespaceHeaderIsEmpty ()
    {
        Assert.Empty(parser.Parse("; \t \t\n").Header);
    }

    [Fact]
    public void CommentOnSecondLineIsNotParsedAsHeader ()
    {
        Assert.Empty(parser.Parse("\n;foo").Header);
    }

    private static object[] Fact (string text, params ManagedTextRecord[] records)
    {
        return new object[] { text, records };
    }
}
