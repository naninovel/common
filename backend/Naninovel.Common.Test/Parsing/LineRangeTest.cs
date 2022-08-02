using System;
using Xunit;

namespace Naninovel.Parsing.Test;

public class LineRangeTest
{
    [Fact]
    public void WhenStartIndexIsBelowZeroExceptionIsThrown ()
    {
        Assert.Throws<ArgumentException>(() => new LineRange(-1, 0));
    }

    [Fact]
    public void WhenLengthIsBelowZeroExceptionIsThrown ()
    {
        Assert.Throws<ArgumentException>(() => new LineRange(0, -1));
    }

    [Fact]
    public void EqualityWorksCorrectly ()
    {
        Assert.False(new LineRange(0, 1).Equals(new LineRange(1, 1)));
        Assert.True(new LineRange(0, 1).Equals((object)new LineRange(0, 1)));
    }

    [Fact]
    public void HashCodeComputedCorrectly ()
    {
        var hash1 = new LineRange(0, 1).GetHashCode();
        var hash2 = new LineRange(1, 1).GetHashCode();
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void EndIndexIsEvaluatedCorrectly ()
    {
        var token = new LineRange(0, 5);
        Assert.Equal(4, token.EndIndex);
    }
}
