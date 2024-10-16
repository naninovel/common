namespace Naninovel.ManagedText.Test;

public class ManagedTextDetectorTest
{
    public static IEnumerable<object[]> Facts { get; } = [
        IsInline(""),
        IsInline("foo: bar"),
        IsInline("; comment"),
        IsInline(" # space before id"),
        IsMultiline("# id"),
        IsMultiline("# id1|id2"),
        IsMultiline("; comment\n# id\nfoo bar baz"),
        IsMultiline("# foo\nauthor: text"),
        IsMultiline("author: text\n# id")
    ];

    [Theory, MemberData(nameof(Facts))]
    public void ParseTheory (string text, bool isMultiline)
    {
        Assert.Equal(isMultiline, ManagedTextDetector.IsMultiline(text));
    }

    private static object[] IsMultiline (string text)
    {
        return [text, true];
    }

    private static object[] IsInline (string text)
    {
        return [text, false];
    }
}
