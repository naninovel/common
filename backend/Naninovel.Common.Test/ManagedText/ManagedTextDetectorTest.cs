using System.Collections.Generic;
using Xunit;

namespace Naninovel.ManagedText.Test;

public class ManagedTextDetectorTest
{
    public static IEnumerable<object[]> Facts { get; } = new[] {
        IsInline(""),
        IsInline("foo: bar"),
        IsInline("; comment"),
        IsInline(" # space before id"),
        IsMultiline("# id"),
        IsMultiline("; comment\n# id\nfoo bar baz"),
        IsMultiline("# foo\nauthor: text"),
        IsMultiline("author: text\n# id")
    };

    [Theory, MemberData(nameof(Facts))]
    public void ParseTheory (string text, bool isMultiline)
    {
        Assert.Equal(isMultiline, ManagedTextDetector.IsMultiline(text));
    }

    private static object[] IsMultiline (string text)
    {
        return new object[] { text, true };
    }

    private static object[] IsInline (string text)
    {
        return new object[] { text, false };
    }
}
