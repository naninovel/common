namespace Naninovel.ManagedText.Test;

public class MultilineManagedTextSerializerTest
{
    public static IEnumerable<object[]> Facts { get; } = new[] {
        Fact("", "", 0, []),
        Fact("", "", 1, []),
        Fact("; header\n", "header", 0, []),
        Fact("; header\n# key\n; comment\nvalue\n", "header", 0, [new("key", "value", "comment")]),
        Fact("; header\n\n# key\n; comment\nvalue\n", "header", 1, [new("key", "value", "comment")]),
        Fact("# key\n<br>\n<br>\n\n", null, 0, [new("key", "<br><br>")]),
        Fact("\n# key1\n<br>\n<br>\n\n\n# key2\n; comment\n\n", null, 1, [new("key1", "<br><br>"), new("key2", "", "comment")]),
        Fact("# k1|k2\n; c1|c2\nv1|v2\n", "", 0, [new("k1", "v1", "c1"), new("k2", "v2", "c2")], [new(0, 1)]),
        Fact("# k1|k2\nv1|v2\n", "", 0, [new("k1", "v1"), new("k2", "v2")], [new(0, 1)]),
        Fact("# k1|k2\n|\n# k3\n; c3\n\n", "", 0, [new("k1", ""), new("k2", ""), new("k3", "", "c3")], [new(0, 1)]),
        Fact("# k1|k2\n; c1|c2\n|\n", "", 0, [new("k1", "", "c1"), new("k2", "", "c2")], [new(0, 1)]),
        Fact("# k1|k2\n; c1|c2\nv1|v2\n# k3|k4\nv3|v4\n", "", 0, [new("k1", "v1", "c1"), new("k2", "v2", "c2"), new("k3", "v3"), new("k4", "v4")], [new(0, 1), new(2, 3)]),
        Fact("# k1|k2\n; c1|c2\nv1|v2\n# k3|k4\nv3|v4\n", "", 0, [new("k1", "v1", "c1"), new("k2", "v2", "c2"), new("k3", "v3"), new("k4", "v4")], [new(2, 3), new(0, 1)]),
        Fact("# k1\nv1\n# k2|k3\nv2|v3\n# k4\nv4\n", "", 0, [new("k1", "v1"), new("k2", "v2"), new("k3", "v3"), new("k4", "v4")], [new(1, 2)]),
        Fact("# k1|k2|k3\n; c1||\n||v3\n", "", 0, [new("k1", "", "c1"), new("k2", ""), new("k3", "v3")], [new(0, 2)]),
        Fact("# k1|k2\n; c\\|1|c2\nv\\|1|v2\n", "", 0, [new("k1", "v|1", "c|1"), new("k2", "v2", "c2")], [new(0, 1)])
    };

    [Theory, MemberData(nameof(Facts))]
    public void ParseTheory (string expected, int indent, ManagedTextDocument document,
        MultilineManagedTextSerializer.JoinRange[] join = null)
    {
        var serializer = new MultilineManagedTextSerializer(indent);
        Assert.Equal(expected, serializer.Serialize(document, join));
    }

    private static object[] Fact (string expected, string header, int indent, ManagedTextRecord[] records,
        MultilineManagedTextSerializer.JoinRange[] join = null)
    {
        return [expected, indent, new ManagedTextDocument(records, header), join];
    }
}
