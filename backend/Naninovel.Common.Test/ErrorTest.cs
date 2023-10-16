namespace Naninovel.Test;

public class ErrorTest
{
    [Fact]
    public async Task CanThrow ()
    {
        await Assert.ThrowsAsync<Error>(() => throw new Error());
        await Assert.ThrowsAsync<Error>(() => throw new Error(""));
        await Assert.ThrowsAsync<Error>(() => throw new Error("", new Error()));
    }
}
