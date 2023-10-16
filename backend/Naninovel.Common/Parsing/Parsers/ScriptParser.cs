using Naninovel.Utilities;

namespace Naninovel.Parsing;

/// <summary>
/// Allows parsing naninovel script text to semantic models.
/// </summary>
public class ScriptParser
{
    private static readonly ParseHandlers nullHandlers = new();

    private readonly Lexer lexer = new();
    private readonly List<Token> tokens = new();
    private readonly CommandLineParser commandParser;
    private readonly CommentLineParser commentParser;
    private readonly GenericLineParser genericParser;
    private readonly LabelLineParser labelParser;

    /// <summary>
    /// Creates a new parser instance.
    /// </summary>
    /// <param name="handlers">Optional parse handlers (hooks).</param>
    public ScriptParser (ParseHandlers? handlers = null)
    {
        handlers ??= nullHandlers;
        commandParser = new(handlers);
        commentParser = new(handlers);
        genericParser = new(handlers);
        labelParser = new(handlers);
    }

    /// <summary>
    /// Splits provided script text into individual lines.
    /// </summary>
    /// <param name="scriptText">The script text to split.</param>
    public static string[] SplitText (string? scriptText)
    {
        return scriptText?.TrimJunk().SplitLines() ?? new[] { string.Empty };
    }

    /// <summary>
    /// Parses provided script text to semantic models.
    /// </summary>
    /// <param name="scriptText">The script text to parse.</param>
    public List<IScriptLine> ParseText (string scriptText)
    {
        var lines = new List<IScriptLine>();
        var textLines = SplitText(scriptText);
        foreach (var textLine in textLines)
            lines.Add(ParseLine(textLine));
        return lines;
    }

    /// <summary>
    /// Parses an individual script text line to the corresponding semantic model.
    /// </summary>
    /// <param name="lineText">The script text line to parse.</param>
    public IScriptLine ParseLine (string lineText)
    {
        tokens.Clear();
        return lexer.TokenizeLine(lineText, tokens) switch {
            LineType.Comment => commentParser.Parse(lineText, tokens),
            LineType.Label => labelParser.Parse(lineText, tokens),
            LineType.Command => commandParser.Parse(lineText, tokens),
            _ => genericParser.Parse(lineText, tokens)
        };
    }
}
