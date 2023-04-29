using System;
using System.Collections.Generic;
using Xunit;

namespace Naninovel.ManagedText.Test;

public class InlineManagedTextSerializerTest
{
    public static IEnumerable<object[]> Facts { get; } = new[] {
        Fact("", "", 0, Array.Empty<ManagedTextRecord>()),
        Fact("", "", 1, Array.Empty<ManagedTextRecord>()),
        Fact("; header\n", "header", 0, Array.Empty<ManagedTextRecord>()),
        Fact("; header\n; comment\nkey: value\n", "header", 0, new ManagedTextRecord("key", "value", "comment")),
        Fact("; header\n\n; comment\nkey: value\n", "header", 1, new ManagedTextRecord("key", "value", "comment")),
        Fact("; header\n\nkey: value\n", "header", 1, new ManagedTextRecord("key", "value", "")),
        Fact("key1: value1\nkey2: value2\n", null, 0, new("key1", "value1"), new("key2", "value2")),
        Fact("\nkey1: value1\n\nkey2: value2\n", null, 1, new("key1", "value1"), new("key2", "value2"))
    };

    [Theory, MemberData(nameof(Facts))]
    public void ParseTheory (string expected, int indent, ManagedTextDocument document)
    {
        var serializer = new InlineManagedTextSerializer(indent);
        Assert.Equal(expected, serializer.Serialize(document));
    }

    private static object[] Fact (string expected, string header, int indent, params ManagedTextRecord[] records)
    {
        return new object[] { expected, indent, new ManagedTextDocument(records, header) };
    }
}
