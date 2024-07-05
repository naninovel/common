namespace Naninovel.ManagedText.Test;

public class MultilineManagedTextSerializerTest
{
    public static TheoryData<string, string, int, ManagedTextRecord[]> Facts { get; } = new() {
        { "", "", 0, [] },
        { "", "", 1, [] },
        { "; header\n", "header", 0, [] },
        { "; header\n# key\n; comment\nvalue\n", "header", 0, [new("key", "value", "comment")] },
        { "; header\n\n# key\n; comment\nvalue\n", "header", 1, [new("key", "value", "comment")] },
        { "# key\n<br>\n<br>\n\n", null, 0, [new("key", "<br><br>")] },
        { "\n# key1\n<br>\n<br>\n\n\n# key2\n; comment\n\n", null, 1, [new("key1", "<br><br>"), new("key2", "", "comment")] },
        { "# k1|k2\n; c1|c2\nv1|v2\n", "", 0, [new("k1|k2", "v1|v2", "c1|c2")] },
    };

    [Theory, MemberData(nameof(Facts))]
    public void ParseTheory (string expected, string header, int indent, ManagedTextRecord[] records)
    {
        var document = new ManagedTextDocument(records, header);
        var serializer = new MultilineManagedTextSerializer(indent);
        Assert.Equal(expected, serializer.Serialize(document));
    }
}
