namespace Naninovel.ManagedText.Test;

public class MultilineManagedTextParserTest
{
    public static TheoryData<string, ManagedTextRecord[]> Facts { get; } = new() {
        { "", [] },
        { "\n\r \n\t", [] },
        { ";", [] },
        { "key: value", [] },
        { "; comment\nkey: value", [] },
        { " # k", [] },
        { "# key", [new("key")] },
        { "# key\n", [new("key")] },
        { "# key \t\n", [new("key")] },
        { "# k \t ey \t\n", [new("k \t ey")] },
        { "# key1\n# key2\n", [new("key1", ""), new("key2", "")] },
        { "# key\n; comment\n", [new("key", "", "comment")] },
        { "# key\n;  comment \t\n", [new("key", "", " comment \t")] },
        { "# key\n; comment\nvalue\n", [new("key", "value", "comment")] },
        { "# key\nvalue\n; comment\n", [new("key", "value", "comment")] },
        { "# key1\nvalue\n; comment\n# key2\n", [new("key1", "value", "comment"), new("key2", "", "")] },
        { "# keyA\na 1 \na2\n# keyB\n b 1\nb2 \n", [new("keyA", "a 1 a2"), new("keyB", " b 1b2 ")] },
        { "# key\nfoo\n\nbar\n", [new("key", "foobar")] },
        { "# key1\n\n\nfoo\nbar\n\n\n# key2\n\n\nvalue\n\n\n", [new("key1", "foobar"), new("key2", "value")] },
        { "# k1|k2\n; c1|c2\nv1|v2\n", [new("k1|k2", "v1|v2", "c1|c2")] },
        { "# k\n; c1\n; c2\n; c3\nv\n", [new("k", "v", "c1\nc2\nc3")] },
        { "# k\n; c1\n; \n; c3\nv\n", [new("k", "v", "c1\n\nc3")] },
        { "#key", [] },
        { "# key\n;value_because_no_space_in_prefix", [new("key", ";value_because_no_space_in_prefix")] },
    };

    private readonly MultilineManagedTextParser parser = new();

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

    [Fact]
    public void ErrsOnEmptyKey ()
    {
        Assert.Throws<Error>(() => parser.Parse("# \n"));
    }
}
