namespace Naninovel.Parsing.Test;

public class ParseErrorTest
{
    [Fact]
    public void CanBeConstructedWithString ()
    {
        var error = new ParseError("foo", 0, 1);
        Assert.Equal("foo", error.Message);
        Assert.Equal(0, error.StartIndex);
        Assert.Equal(1, error.Length);
        Assert.Equal(0, error.EndIndex);
    }

    [Fact]
    public void CanBeConstructedWithToken ()
    {
        var error = new ParseError(new Token(ErrorType.MissingLabel, 0, 1));
        Assert.Equal(LexingErrors.GetFor(ErrorType.MissingLabel), error.Message);
        Assert.Equal(0, error.StartIndex);
        Assert.Equal(1, error.Length);
        Assert.Equal(0, error.EndIndex);
    }
}
