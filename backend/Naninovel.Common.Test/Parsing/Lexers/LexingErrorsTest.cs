namespace Naninovel.Parsing.Test;

public class LexingErrorsTest
{
    [Fact]
    public void ValidErrorDescriptionIsReturned ()
    {
        const string expected = "Parameter identifier is missing.";
        Assert.Equal(expected, LexingErrors.GetFor(ErrorType.MissingParamId));
    }
}
