using System.Collections.Generic;
using Xunit;

namespace Naninovel.Parsing.Test;

public abstract class LineParserTest<TParser, TLine>
    where TLine : LineContent, IScriptLine, new()
    where TParser : LineParser<TLine>, new()
{
    protected readonly TParser Parser = new();
    protected abstract string ExampleLine { get; }

    private readonly Lexer lexer = new();
    private readonly List<Token> tokens = new();
    private readonly List<ParseError> errors = new();

    [Fact]
    public void WhenReturnedLineIsReused ()
    {
        var line1 = Parse(ExampleLine);
        Parser.ReturnLine(line1);
        var line2 = Parse(ExampleLine);
        Assert.Equal(line1, line2);
    }

    [Fact]
    public void StartIndexIsZero ()
    {
        Assert.Equal(0, Parse(ExampleLine).StartIndex);
    }

    [Fact]
    public void LengthEqualsTextLength ()
    {
        Assert.Equal(ExampleLine.Length, Parse(ExampleLine).Length);
    }

    protected TLine Parse (string lineText)
    {
        errors.Clear();
        tokens.Clear();
        lexer.TokenizeLine(lineText, tokens);
        return Parser.Parse(lineText, tokens, errors);
    }

    protected bool HasError (ErrorType error)
    {
        return errors.Exists(e => e.Message == LexingErrors.GetFor(error));
    }

    protected bool HasError (string error)
    {
        return errors.Exists(e => e.Message == error);
    }
}
