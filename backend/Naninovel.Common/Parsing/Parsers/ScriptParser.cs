using System;
using System.Collections.Generic;

namespace Naninovel.Parsing;

/// <summary>
/// Allows parsing naninovel script text to semantic models.
/// </summary>
public class ScriptParser
{
    private static readonly string[] lineBreakSymbols = { "\r\n", "\n", "\r" };

    private readonly Lexer lexer = new();
    private readonly List<Token> tokens = new();
    private readonly CommandLineParser commandParser = new();
    private readonly CommentLineParser commentParser = new();
    private readonly GenericLineParser genericParser = new();
    private readonly LabelLineParser labelParser = new();

    /// <summary>
    /// Splits provided script text into individual lines.
    /// </summary>
    /// <param name="scriptText">The script text to split.</param>
    public static string[] SplitText (string scriptText)
    {
        return scriptText?.Trim('\uFEFF', '\u200B') // Remove BOM and zero-width space.
            .Split(lineBreakSymbols, StringSplitOptions.None) ?? new[] { string.Empty };
    }

    /// <summary>
    /// Parses provided script text to semantic models.
    /// </summary>
    /// <param name="scriptText">The script text to parse.</param>
    /// <param name="errors">When provided, will add parse errors to the collection.</param>
    public List<IScriptLine> ParseText (string scriptText, ICollection<ParseError> errors = default)
    {
        var lines = new List<IScriptLine>();
        var textLines = SplitText(scriptText);
        foreach (var textLine in textLines)
            lines.Add(ParseLine(textLine, errors));
        return lines;
    }

    /// <summary>
    /// Parses an individual script text line to the corresponding semantic model.
    /// </summary>
    /// <param name="lineText">The script text line to parse.</param>
    /// <param name="errors">When provided, will add parse errors to the collection.</param>
    public IScriptLine ParseLine (string lineText, ICollection<ParseError> errors = default)
    {
        tokens.Clear();
        return lexer.TokenizeLine(lineText, tokens) switch {
            LineType.Comment => commentParser.Parse(lineText, tokens, errors),
            LineType.Label => labelParser.Parse(lineText, tokens, errors),
            LineType.Command => commandParser.Parse(lineText, tokens, errors),
            _ => genericParser.Parse(lineText, tokens, errors),
        };
    }
}
