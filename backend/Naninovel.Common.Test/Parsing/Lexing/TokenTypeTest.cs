using Xunit;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing.Test;

public class TokenTypeTest
{
    [Fact]
    public void HasFlagEvaluatedCorrectly ()
    {
        Assert.True(TokenTypeExtensions.HasFlag(TokenType.Expression, TokenType.Expression));
        Assert.False(TokenTypeExtensions.HasFlag(LineId, TokenType.GenericText));
        Assert.True(TokenTypeExtensions.HasFlag(LineId | TokenType.Expression | CommentText, TokenType.Expression));
        Assert.False(TokenTypeExtensions.HasFlag(CommandBody | AuthorAssign, LineId));
    }
}
