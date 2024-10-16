using Naninovel.Metadata;
using Naninovel.Parsing;

namespace Naninovel.TestUtilities.Test;

public class MetadataMockTest
{
    private readonly MetadataMock meta = new();

    [Fact]
    public void CanSetupNavigationCommands ()
    {
        var resolver = new EndpointResolver(meta);
        meta.SetupNavigationCommands();
        Assert.True(resolver.TryResolve(new("goto", [new(new([new PlainText("foo.bar")]))]), out var goPoint));
        Assert.Equal("foo", goPoint.ScriptPath);
        Assert.Equal("bar", goPoint.Label);
        Assert.True(resolver.TryResolve(new("gosub", [new(new([new PlainText("foo.bar")]))]), out var subPoint));
        Assert.Equal("foo", subPoint.ScriptPath);
        Assert.Equal("bar", subPoint.Label);
        Assert.True(meta.FindCommand("gosub")!.Branch!.Traits.HasFlag(BranchTraits.Return));
    }

    [Fact]
    public void CanSetupSwitchCommands ()
    {
        var resolver = new ConditionResolver(meta);
        meta.SetupSwitchCommands();
        var ifParam = new Parsing.Parameter(new([new PlainText("x")]));
        var ifCommand = new Parsing.Command("if", [ifParam]);
        var elseParam = new Parsing.Parameter(new([new PlainText("y")]));
        var elseCommand = new Parsing.Command("else", [elseParam]);
        var script = new IScriptLine[] {
            new LabelLine(new("Label")),
            new CommandLine(ifCommand),
            new GenericLine([], 1),
            new CommandLine(elseCommand),
            new CommandLine(new("do"), 1)
        };
        using var _ = ListPool<Condition>.Rent(out var conditions);
        Assert.True(resolver.TryResolve(((CommandLine)script[4]).Command, script, conditions));
        Assert.Equal([new(script[3], elseCommand, elseParam), new(script[1], ifCommand, ifParam, true)], conditions);
    }
}
