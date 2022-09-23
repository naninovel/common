using Xunit;
using static Naninovel.Parsing.ErrorType;

namespace Naninovel.Parsing.Test;

public class ErrorTypeTest
{
    [Fact]
    public void HasFlagEvaluatedCorrectly ()
    {
        Assert.True(ErrorTypeExtensions.HasFlag(MissingAppearance, MissingAppearance));
        Assert.False(ErrorTypeExtensions.HasFlag(SpaceInLabel, MissingAppearance));
        Assert.True(ErrorTypeExtensions.HasFlag(MissingParamId | MissingLabel | MissingCommandId, MissingLabel));
        Assert.False(ErrorTypeExtensions.HasFlag(SpaceInLabel | MultipleNameless, MissingParamValue));
    }
}