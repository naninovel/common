using Moq;

namespace Naninovel.Parsing.Test;

public class ErrorHandlerTest
{
    private readonly Lexer lexer = new();
    private readonly List<Token> tokens = [];

    [Fact]
    public void WhenHandlerNotAssignedErrorIsNotHandled ()
    {
        lexer.TokenizeLine("foo", tokens);
        new LabelLineParser(new()).Parse("foo", tokens);
        tokens.Clear();
        lexer.TokenizeLine("# foo bar", tokens);
        new LabelLineParser(new()).Parse("# foo bar", tokens);
    }

    [Fact]
    public void WhenHandlerAssignedErrorIsNotHandled ()
    {
        var handler = new Mock<IErrorHandler>();
        lexer.TokenizeLine("foo", tokens);
        new LabelLineParser(new() { Handlers = new() { ErrorHandler = handler.Object } }).Parse("foo", tokens);
        tokens.Clear();
        lexer.TokenizeLine("# foo bar", tokens);
        new LabelLineParser(new() { Handlers = new() { ErrorHandler = handler.Object } }).Parse("# foo bar", tokens);
        handler.Verify(h => h.HandleError(It.IsAny<ParseError>()), Times.Exactly(2));
    }
}
