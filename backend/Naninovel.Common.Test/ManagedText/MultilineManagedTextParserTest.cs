﻿using System;
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
        Fact("#key", new ManagedTextRecord("key")),
        Fact("# key \t", new ManagedTextRecord("key")),
        Fact("# k \t ey \t", new ManagedTextRecord("k \t ey")),
        Fact("#key1\n#key2", new("key1", ""), new("key2", "")),
        Fact("# key\n; comment", new ManagedTextRecord("key", "", "comment")),
        Fact("# key\n; comment\nvalue", new ManagedTextRecord("key", "value", "comment")),
        Fact("# key\nvalue\n; comment", new ManagedTextRecord("key", "value", "comment")),
        Fact("# key1\nvalue\n; comment\n# key2", new("key1", "value", "comment"), new("key2", "", "")),
        Fact("# key\nfoo\nbar\nnya", new ManagedTextRecord("key", "foo<br>bar<br>nya")),
        Fact("# keyA\na 1 \na2\n# keyB\n b 1\nb2 ", new("keyA", "a 1 <br>a2"), new("keyB", " b 1<br>b2 ")),
        Fact("# key\nfoo\n\nbar", new ManagedTextRecord("key", "foo<br><br>bar"))
    };

    [Theory, MemberData(nameof(Facts))]
    public void ParseTheory (string text, params ManagedTextRecord[] expected)
    {
        var parser = new MultilineManagedTextParser();
        var records = parser.Parse(text).Records.ToArray();
        Assert.Equal(expected.Length, records.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i].Key, records[i].Key);
            Assert.Equal(expected[i].Value, records[i].Value);
            Assert.Equal(expected[i].Comment, records[i].Comment);
        }
    }

    private static object[] Fact (string text, params ManagedTextRecord[] records)
    {
        return new object[] { text, records };
    }
}