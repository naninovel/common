using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace Naninovel.Parsing.Test;

public class LexerTest
{
    private static readonly Lexer lexer = new();
    private readonly ITestOutputHelper output;

    public LexerTest (ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void NullLineThrowsException ()
    {
        Assert.Throws<ArgumentNullException>(() => lexer.TokenizeLine(null, new List<Token>()));
    }

    [Fact]
    public void NullTokensThrowsException ()
    {
        Assert.Throws<ArgumentNullException>(() => lexer.TokenizeLine("", null));
    }

    [Fact]
    public void ReadOnlyTokensThrowsException ()
    {
        Assert.Throws<ArgumentException>(() => lexer.TokenizeLine("", new Token[1]));
    }

    [Fact]
    public void CommandBodyTokenized ()
    {
        var tokens = new List<Token>();
        lexer.TokenizeCommandBody("x y:z", tokens);
        Assert.Equal(6, tokens.Count);
        Assert.Equal(new Token(TokenType.CommandId, 0, 1), tokens[0]);
        Assert.Equal(new Token(TokenType.ParamId, 2, 1), tokens[1]);
        Assert.Equal(new Token(TokenType.ParamAssign, 3, 1), tokens[2]);
        Assert.Equal(new Token(TokenType.ParamValue, 4, 1), tokens[3]);
        Assert.Equal(new Token(TokenType.NamedParam, 2, 3), tokens[4]);
        Assert.Equal(new Token(TokenType.CommandBody, 0, 5), tokens[5]);
    }

    [Theory, MemberData(nameof(LexerTestData.CommentLines), MemberType = typeof(LexerTestData))]
    public void CommentLineTokenized (string text, params Token[] tokens) => LineTokenized(text, LineType.Comment, tokens);

    [Theory, MemberData(nameof(LexerTestData.LabelLines), MemberType = typeof(LexerTestData))]
    public void LabelLineTokenized (string text, params Token[] tokens) => LineTokenized(text, LineType.Label, tokens);

    [Theory, MemberData(nameof(LexerTestData.CommandLines), MemberType = typeof(LexerTestData))]
    public void CommandLineTokenized (string text, params Token[] tokens) => LineTokenized(text, LineType.Command, tokens);

    [Theory, MemberData(nameof(LexerTestData.GenericLines), MemberType = typeof(LexerTestData))]
    public void GenericLineTokenized (string text, params Token[] tokens) => LineTokenized(text, LineType.Generic, tokens);

    [ExcludeFromCodeCoverage]
    private void LineTokenized (string text, LineType expectedLineType, params Token[] expectedTokens)
    {
        var tokens = new List<Token>();
        var lineType = lexer.TokenizeLine(text, tokens);
        PrintTokens();
        Assert.Equal(expectedLineType, lineType);
        Assert.Equal(expectedTokens.Length, tokens.Count);
        for (int i = 0; i < expectedTokens.Length; i++)
            Assert.Equal(expectedTokens[i], tokens[i]);

        void PrintTokens ()
        {
            output.WriteLine($"Expected: {string.Join(' ', expectedTokens)}");
            output.WriteLine($"Actual: {string.Join(' ', tokens)}");
        }
    }
}
