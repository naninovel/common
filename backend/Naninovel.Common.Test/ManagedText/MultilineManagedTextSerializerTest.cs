namespace Naninovel.ManagedText.Test;

public class MultilineManagedTextSerializerTest
{
    public static IEnumerable<object[]> Facts { get; } = new[] {
        Fact("", "", 0, Array.Empty<ManagedTextRecord>()),
        Fact("", "", 1, Array.Empty<ManagedTextRecord>()),
        Fact("; header\n", "header", 0, Array.Empty<ManagedTextRecord>()),
        Fact("; header\n# key\n; comment\nvalue\n", "header", 0, new ManagedTextRecord("key", "value", "comment")),
        Fact("; header\n\n# key\n; comment\nvalue\n", "header", 1, new ManagedTextRecord("key", "value", "comment")),
        Fact("# key\n<br>\n<br>\n\n", null, 0, new ManagedTextRecord("key", "<br><br>")),
        Fact("\n# key1\n<br>\n<br>\n\n\n# key2\n; comment\n\n", null, 1, new("key1", "<br><br>"), new("key2", "", "comment")),
    };

    [Theory, MemberData(nameof(Facts))]
    public void ParseTheory (string expected, int indent, ManagedTextDocument document)
    {
        var serializer = new MultilineManagedTextSerializer(indent);
        Assert.Equal(expected, serializer.Serialize(document));
    }

    private static object[] Fact (string expected, string header, int indent, params ManagedTextRecord[] records)
    {
        return new object[] { expected, indent, new ManagedTextDocument(records, header) };
    }
}
