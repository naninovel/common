using System;

namespace Naninovel.ManagedText;

/// <summary>
/// Allows creating <see cref="ManagedTextDocument"/> from serialized text in multiline format.
/// </summary>
/// <remarks>
/// Multiline format spec:
/// <code>
/// # key (space around key is ignored)
/// ; comment (optional, space around comment is ignored)
/// value (all lines until next key are joined with br tag, space is preserved)
/// </code>
/// </remarks>
public class MultilineManagedTextParser
{
    /// <summary>
    /// Creates document from specified serialized text string.
    /// </summary>
    public ManagedTextDocument Parse (string text)
    {
        return new ManagedTextDocument(Array.Empty<ManagedTextRecord>());
    }
}
