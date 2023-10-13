using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Naninovel.ManagedText.Test;

public class InlineManagedTextParserTest
{
    public static IEnumerable<object[]> Facts { get; } = new[] {
        Fact("", Array.Empty<ManagedTextRecord>()),
        Fact("\n\r \n\t", Array.Empty<ManagedTextRecord>()),
        Fact(":", Array.Empty<ManagedTextRecord>()),
        Fact(":x", Array.Empty<ManagedTextRecord>()),
        Fact(" : x\n", Array.Empty<ManagedTextRecord>()),
        Fact(";", Array.Empty<ManagedTextRecord>()),
        Fact(";x\n:x", Array.Empty<ManagedTextRecord>()),
        Fact("x:\n;x", Array.Empty<ManagedTextRecord>()),
        Fact("key: value", new ManagedTextRecord("key", "value")),
        Fact("key: ", new ManagedTextRecord("key", "")),
        Fact(" ke \t y : value", new ManagedTextRecord(" ke \t y ", "value")),
        Fact("key:  \tvalue \t<br>\t", new ManagedTextRecord("key", " \tvalue \t<br>\t")),
        Fact("; comment\nkey: value", new ManagedTextRecord("key", "value", "comment")),
        Fact(";foo\n;bar\nkey: value", new ManagedTextRecord("key", "value", "bar")),
        Fact("; comment\tkey: value", Array.Empty<ManagedTextRecord>()),
        Fact(";\nkey: value", new ManagedTextRecord("key", "value", "")),
        Fact(";  \t comment\t \t\nkey: \tvalue\t", new ManagedTextRecord("key", "\tvalue\t", " \t comment\t \t")),
        Fact("; comment 1\nkey1: value1\n; comment 2\nkey2: value2", new("key1", "value1", "comment 1"), new("key2", "value2", "comment 2")),
        Fact("; foo\n; bar\nkey: value", new ManagedTextRecord("key", "value", "bar")),
        Fact("; comment1\nkey1: value1\nkey2: value2\n; comment2", new("key1", "value1", "comment1"), new("key2", "value2", "")),
        Fact("text1\nkey1: value1\ntext2\nkey2: value2", new("key1", "value1", ""), new("key2", "value2", "")),
    };

    private readonly InlineManagedTextParser parser = new();

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
        Assert.Equal("\tfoo \t", parser.Parse("; \tfoo \t").Header);
    }

    [Fact]
    public void WhenCommentOnFirstLineIsWhitespaceHeaderIsNotEmpty ()
    {
        Assert.Equal("\t \t", parser.Parse("; \t \t\n").Header);
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
