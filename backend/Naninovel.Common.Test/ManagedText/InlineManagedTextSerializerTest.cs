namespace Naninovel.ManagedText.Test;

public class InlineManagedTextSerializerTest
{
    public static TheoryData<string, int, ManagedTextRecord[], string> Facts { get; } = new() {
        { "", 0, [], "\n" },
        { "", 1, [], "\n" },
        { "header", 0, [], "; header\n" },
        { "header", 0, [new("key", "value", "comment")], "; header\n; comment\nkey: value\n" },
        { "header", 1, [new("key", "value", "comment")], "; header\n\n; comment\nkey: value\n" },
        { "header", 1, [new("key", "value", "")], "; header\n\nkey: value\n" },
        { null, 0, [new("key1", "value1"), new("key2", "value2")], "key1: value1\nkey2: value2\n" },
        { null, 1, [new("key1", "value1"), new("key2", "value2")], "\nkey1: value1\n\nkey2: value2\n" },
        { null, 0, [new("key", "value", "comment1\ncomment2")], "\n; comment1\n; comment2\nkey: value\n" },
        { null, 0, [new("key", "value", "comment1\n\ncomment3")], "\n; comment1\n; \n; comment3\nkey: value\n" }
    };

    [Theory, MemberData(nameof(Facts))]
    public void ParseTheory (string header, int indent, ManagedTextRecord[] records, string expected)
    {
        var serializer = new InlineManagedTextSerializer(indent);
        var document = new ManagedTextDocument(records, header);
        Assert.Equal(expected, serializer.Serialize(document));
    }
}
