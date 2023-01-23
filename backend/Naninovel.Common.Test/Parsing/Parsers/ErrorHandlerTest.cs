using System.Collections.Generic;
using Moq;
using Xunit;

namespace Naninovel.Parsing.Test;

public class ErrorHandlerTest
{
    private readonly Lexer lexer = new();
    private readonly List<Token> tokens = new();

    [Fact]
    public void WhenHandlerNotAssignedErrorIsNotHandled ()
    {
        lexer.TokenizeLine("foo", tokens);
        new LabelLineParser().Parse("foo", tokens);
        tokens.Clear();
        lexer.TokenizeLine("# foo bar", tokens);
        new LabelLineParser().Parse("# foo bar", tokens);
    }

    [Fact]
    public void WhenHandlerAssignedErrorIsNotHandled ()
    {
        var handler = new Mock<IErrorHandler>();
        lexer.TokenizeLine("foo", tokens);
        new LabelLineParser(handler.Object).Parse("foo", tokens);
        tokens.Clear();
        lexer.TokenizeLine("# foo bar", tokens);
        new LabelLineParser(handler.Object).Parse("# foo bar", tokens);
        handler.Verify(h => h.HandleError(It.IsAny<ParseError>()), Times.Exactly(2));
    }
}
