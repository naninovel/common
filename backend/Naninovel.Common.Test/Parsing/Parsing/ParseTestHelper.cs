using System;
using System.Collections.Generic;
using System.Linq;

namespace Naninovel.Parsing.Test;

public class ParseTestHelper<TLine> where TLine : IScriptLine
{
    public List<Token> Tokens { get; } = new();
    public List<ParseError> Errors { get; } = new();

    private readonly Func<string, IReadOnlyList<Token>, ICollection<ParseError>, TLine> parse;
    private readonly Lexer lexer = new();

    public ParseTestHelper (Func<string, IReadOnlyList<Token>, ICollection<ParseError>, TLine> parse)
    {
        this.parse = parse;
    }

    public TLine Parse (string lineText)
    {
        Tokens.Clear();
        lexer.TokenizeLine(lineText, Tokens);
        return parse(lineText, Tokens, Errors);
    }

    public bool HasError (string message)
    {
        return Errors.Any(e => e.Message == message);
    }

    public bool HasError (ErrorType error)
    {
        return Errors.Any(e => e.Message == LexingErrors.GetFor(error));
    }
}
