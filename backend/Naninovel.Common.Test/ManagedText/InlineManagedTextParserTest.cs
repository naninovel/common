namespace Naninovel.ManagedText.Test;

public class InlineManagedTextParserTest
{
    public static TheoryData<string, ManagedTextRecord[]> Facts { get; } = new() {
        { "", [] },
        { "\n\r \n\t", [] },
        { ":", [] },
        { ":x", [] },
        { " : x\n", [] },
        { "\n;", [] },
        { "\n; x\n:x", [] },
        { "x:\n; x", [] },
        { "key: value", [new("key", "value")] },
        { "key: ", [new("key", "")] },
        { " ke \t y : value", [new(" ke \t y ", "value")] },
        { "key:  \tvalue \t<br>\t", [new("key", " \tvalue \t<br>\t")] },
        { "\n; comment\nkey: value", [new("key", "value", "comment")] },
        { "; header\n; comment\nkey: value", [new("key", "value", "comment")] },
        { "\n; comment\tkey: value", [] },
        { "\n; \nkey: value", [new("key", "value", "")] },
        { "\n;  \t comment\t \t\nkey: \tvalue\t", [new("key", "\tvalue\t", " \t comment\t \t")] },
        { "\n; comment 1\nkey1: value1\n; comment 2\nkey2: value2", [new("key1", "value1", "comment 1"), new("key2", "value2", "comment 2")] },
        { "\n; comment1\nkey1: value1\nkey2: value2\n; comment2", [new("key1", "value1", "comment1"), new("key2", "value2", "")] },
        { "text\nkey1: value1\ntext2\nkey2: value2", [new("key1", "value1", ""), new("key2", "value2", "")] },
        { "\n; comment1\n; comment2\nkey: value", [new("key", "value", "comment1\ncomment2")] }
    };

    private readonly InlineManagedTextParser parser = new();

    [Theory, MemberData(nameof(Facts))]
    public void ParseTheory (string text, ManagedTextRecord[] expected)
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
        Assert.Equal("foo", parser.Parse("; foo").Header);
    }

    [Fact]
    public void HeaderIsNotTrimmed ()
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
        Assert.Empty(parser.Parse("\n; foo").Header);
    }
}
