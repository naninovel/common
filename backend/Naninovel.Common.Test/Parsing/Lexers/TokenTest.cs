using Xunit;

namespace Naninovel.Parsing.Test;

public class TokenTest
{
    [Fact]
    public void EqualityWorksCorrectly ()
    {
        Assert.False(new Token(TokenType.Inlined, 0, 1).Equals(new Token(TokenType.Inlined, 1, 1)));
        Assert.True(new Token(TokenType.Inlined, 0, 1).Equals((object)new Token(TokenType.Inlined, 0, 1)));
    }

    [Fact]
    public void HashCodeComputedCorrectly ()
    {
        var hash1 = new Token(TokenType.Inlined, 0, 1).GetHashCode();
        var hash2 = new Token(TokenType.Inlined, 1, 1).GetHashCode();
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void ErrorEvaluatedCorrectly ()
    {
        var token = new Token(ErrorType.MissingLabel, 5, 1);
        Assert.True(token.IsError(ErrorType.MissingLabel));
        Assert.False(token.IsError(ErrorType.MultipleNameless));
    }
}
