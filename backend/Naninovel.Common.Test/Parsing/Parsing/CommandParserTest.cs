using Xunit;

namespace Naninovel.Parsing.Test;

public class CommandParserTest
{
    private readonly CommandParser parser = new();

    [Fact]
    public void CommandBodyParsed ()
    {
        var command = parser.GetCommand();
        parser.ParseBody("x y:z", command);
        Assert.Equal("x", command.Identifier);
        Assert.Equal("y", command.Parameters[0].Identifier);
        Assert.Equal("z", command.Parameters[0].Value);
    }

    [Fact]
    public void WhenParsingCommandBodyModelIsReused ()
    {
        var command1 = parser.GetCommand();
        parser.ParseBody("x", command1);
        parser.ReturnCommand(command1);
        var command2 = parser.GetCommand();
        parser.ParseBody("y", command2);
        Assert.Equal("y", command2.Identifier);
        Assert.Equal(command1, command2);
    }

    [Fact]
    public void PartialParameterParsed ()
    {
        var command = parser.GetCommand();
        parser.ParseBody("c p:", command);
        Assert.Equal("p", command.Parameters[0].Identifier);
    }

    [Fact]
    public void ExpressionsAreClearedBetweenRuns ()
    {
        var command = parser.GetCommand();
        parser.ParseBody("cmd x {x}", command);
        parser.ParseBody("cmd x {}", command);
        Assert.Equal("cmd", command.Identifier);
        Assert.Equal("x", command.Parameters[0].Value);
    }
}
