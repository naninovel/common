using Xunit;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing.Test;

public class TokenTypeTest
{
    [Fact]
    public void HasFlagEvaluatedCorrectly ()
    {
        Assert.True(TokenTypeExtensions.HasFlag(Expression, Expression));
        Assert.False(TokenTypeExtensions.HasFlag(LineId, TokenType.GenericText));
        Assert.True(TokenTypeExtensions.HasFlag(LineId | Expression | CommentText, Expression));
        Assert.False(TokenTypeExtensions.HasFlag(CommandBody | AuthorAssign, LineId));
    }
}