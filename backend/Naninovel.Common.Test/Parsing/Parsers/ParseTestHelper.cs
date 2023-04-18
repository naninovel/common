using System;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace Naninovel.Parsing.Test;

public class ParseTestHelper<TLine> where TLine : IScriptLine
{
    public List<Token> Tokens { get; } = new();
    public List<ParseError> Errors { get; } = new();
    public Dictionary<ILineComponent, LineRange> Associations { get; } = new();

    private readonly Func<string, IReadOnlyList<Token>, TLine> parse;
    private readonly Lexer lexer = new();

    public ParseTestHelper (Func<IErrorHandler, IRangeAssociator, Func<string, IReadOnlyList<Token>, TLine>> ctor)
    {
        var errorHandler = new Mock<IErrorHandler>();
        errorHandler.Setup(h => h.HandleError(Capture.In(Errors)));
        var associator = new Mock<IRangeAssociator>();
        associator.Setup(a => a.Associate(It.IsAny<ILineComponent>(), It.IsAny<LineRange>())).Callback(Associations.Add);
        parse = ctor(errorHandler.Object, associator.Object);
    }

    public TLine Parse (string lineText)
    {
        Tokens.Clear();
        lexer.TokenizeLine(lineText, Tokens);
        return parse(lineText, Tokens);
    }

    public bool HasError (string message)
    {
        return Errors.Any(e => e.Message == message);
    }

    public bool HasError (ErrorType error)
    {
        return Errors.Any(e => e.Message == LexingErrors.GetFor(error));
    }

    public LineRange? Resolve (ILineComponent component)
    {
        return Associations.TryGetValue(component, out var range) ? range : null;
    }
}
