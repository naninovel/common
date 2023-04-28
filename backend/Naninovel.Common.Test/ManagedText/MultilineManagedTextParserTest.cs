using System;
using System.Collections.Generic;
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
        // Fact("# key", new ManagedTextRecord("key")),
        // Fact("#key", new ManagedTextRecord("key")),
        // Fact("# key \t", new ManagedTextRecord("key")),
        // Fact("# k \t ey \t", new ManagedTextRecord("k \t ey")),
        // Fact("#key1\n#key2", new("key1", ""), new("key2", "")),
        // Fact("# key\n; comment", new ManagedTextRecord("key", "", "comment")),
        // Fact("# key\n; comment\nvalue", new ManagedTextRecord("key", "value", "comment")),
        // Fact("# key\nvalue\n; comment", new ManagedTextRecord("key", "value", "")),
        // Fact("# key1\nvalue\n; comment\n# key2", new("key1", "value", ""), new("key2", "", "")),
        // Fact("# key\nfoo \nbar \nnya", new ManagedTextRecord("key", "foo bar nya")),
        // Fact("# keyA\na1\na2\n# keyB\nb1\nb2", new("keyA", "a1a2"), new("keyB", "b1b2"))
    };

    [Theory, MemberData(nameof(Facts))]
    public void ParseTheory (string text, params ManagedTextRecord[] expected)
    {
        var parser = new MultilineManagedTextParser();
        Assert.Equal(expected, parser.Parse(text).Records);
    }

    private static object[] Fact (string text, params ManagedTextRecord[] records)
    {
        return new object[] { text, records };
    }
}
