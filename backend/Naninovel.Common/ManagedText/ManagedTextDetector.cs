using static Naninovel.ManagedText.ManagedTextConstants;

namespace Naninovel.ManagedText;

/// <summary>
/// Allows checking whether provided serialized managed text document string
/// is in multiline or inline format.
/// </summary>
public static class ManagedTextDetector
{
    public static bool IsMultiline (string text)
    {
        foreach (var line in text.IterateLines())
            if (line.StartsWithOrdinal(RecordMultilineKeyLiteral))
                return true;
        return false;
    }
}
