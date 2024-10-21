using Naninovel.Parsing;

namespace Naninovel.Metadata.Test;

public class ConditionResolverTest
{
    private readonly ConditionResolver resolver;
    private readonly MetadataProvider meta = new();

    public ConditionResolverTest ()
    {
        resolver = new(meta);
    }

    [Fact]
    public void ConditionHashingIsCorrect ()
    {
        var line = new CommandLine(new("c", [new("p", new MixedValue([]))]));
        Assert.True(new Condition(line, line.Command, line.Command.Parameters[0])
            .Equals(new Condition(line, line.Command, line.Command.Parameters[0])));
        Assert.False(new Condition(line, line.Command, line.Command.Parameters[0])
            .Equals(new Condition(line, line.Command)));
        Assert.False(new Condition().Equals(null));
        Assert.Equal(new Condition(line, line.Command, line.Command.Parameters[0]).GetHashCode(),
            new Condition(line, line.Command, line.Command.Parameters[0]).GetHashCode());
    }

    [Fact]
    public void HasFlagEvaluatedCorrectly ()
    {
        Assert.True(BranchFlagsExtensions.HasFlag(BranchTraits.Nest, BranchTraits.Nest));
        Assert.False(BranchFlagsExtensions.HasFlag(BranchTraits.Endpoint, BranchTraits.Nest));
        Assert.True(BranchFlagsExtensions.HasFlag(BranchTraits.Return | BranchTraits.Nest | BranchTraits.Endpoint, BranchTraits.Nest));
        Assert.False(BranchFlagsExtensions.HasFlag(BranchTraits.Return | BranchTraits.Endpoint, BranchTraits.Nest));
    }

    [Fact]
    public void WhenUnknownCommandReturnsFalse ()
    {
        Assert.False(resolver.TryResolve(new("c"), out _));
    }

    [Fact]
    public void WhenUnknownParameterReturnsFalse ()
    {
        meta.Update(new() {
            Commands = [new() { Id = "c" }]
        });
        Assert.False(resolver.TryResolve(new("c"), out _));
    }

    [Fact]
    public void WhenNonConditionParameterReturnsFalse ()
    {
        meta.Update(new() {
            Commands = [new() { Id = "c", Parameters = [new() { Id = "p" }] }]
        });
        Assert.False(resolver.TryResolve(new("c", [new("p", new[] { new Parsing.Expression("e") })]), out _));
    }

    [Fact]
    public void CanResolveByParamId ()
    {
        meta.Update(new() {
            Commands = [new() { Id = "c", Parameters = [CreateConditionParamMeta("p")] }]
        });
        Assert.True(resolver.IsCondition("p", "c"));
    }

    [Fact]
    public void CanResolveInCommand ()
    {
        meta.Update(new() {
            Commands = [new() { Id = "c", Parameters = [CreateConditionParamMeta("p")] }]
        });
        var cmd = new Parsing.Command("c", [new("p", new[] { new Parsing.Expression("e") })]);
        Assert.True(resolver.TryResolve(cmd, out var param));
        Assert.Equal(cmd.Parameters[0], param);
    }

    [Fact]
    public void CanResolveInScript ()
    {
        meta.Update(new() {
            Commands = [
                CreateConditionalCommand("if", "c"),
                new() { Id = "do", Parameters = [CreateConditionParamMeta("c")] }
            ]
        });
        var ifLine = new CommandLine(new("if", [new("c", new[] { new Parsing.Expression("e1") })]));
        var doLine = new CommandLine(new("do", [new("c", new[] { new Parsing.Expression("e2") })]), 1);
        var script = new IScriptLine[] { ifLine, doLine };
        var resolved = new List<Condition>();
        Assert.True(resolver.TryResolve(((CommandLine)script[1]).Command, script, resolved));
        Assert.Equal([
            new(doLine, doLine.Command, doLine.Command.Parameters[0]),
            new(ifLine, ifLine.Command, ifLine.Command.Parameters[0])
        ], resolved);
    }

    [Fact]
    public void CanResolveSingleLineInScript ()
    {
        meta.Update(new() {
            Commands = [
                new() { Id = "do", Parameters = [CreateConditionParamMeta("c")] },
            ]
        });
        var doLine = new CommandLine(new("do", [new("c", new[] { new Parsing.Expression("e") })]));
        var script = new IScriptLine[] { doLine };
        var resolved = new List<Condition>();
        Assert.True(resolver.TryResolve(doLine.Command, script, resolved));
        Assert.Equal([
            new(doLine, doLine.Command, doLine.Command.Parameters[0])
        ], resolved);
    }

    [Fact]
    public void ThrowsWhenCommandNotFoundInScript ()
    {
        meta.Update(new() {
            Commands = [
                CreateConditionalCommand("if", "c"),
                new() { Id = "do", Parameters = [CreateConditionParamMeta("c")] }
            ]
        });
        var script = new IScriptLine[] {
            new CommandLine(new("if", [new("c", new[] { new Parsing.Expression("e1") })])),
            new CommandLine(new("do", [new("c", new[] { new Parsing.Expression("e2") })]), 1),
        };
        var resolved = new List<Condition>();
        Assert.Contains("command not found",
            Assert.Throws<Error>(() => resolver.TryResolve(((CommandLine)script[1]).Command, script[..^1], resolved)).Message);
    }

    [Fact]
    public void CanResolveConditionsForInlinedCommand ()
    {
        meta.Update(new() {
            Commands = [
                CreateConditionalCommand("if", "c"),
                new() { Id = "do", Parameters = [CreateConditionParamMeta("c")] }
            ]
        });
        var inlined = new Parsing.Command("do");
        var ifLine = new CommandLine(new("if", [new("c", new[] { new Parsing.Expression("e") })]));
        var textLine = new GenericLine([new MixedValue([new PlainText("x")]), new InlinedCommand(inlined), new MixedValue([new PlainText("x")])], 1);
        var script = new IScriptLine[] { ifLine, textLine };
        var resolved = new List<Condition>();
        Assert.True(resolver.TryResolve(inlined, script, resolved));
        Assert.Equal([
            new(ifLine, ifLine.Command, ifLine.Command.Parameters[0])
        ], resolved);
    }

    [Fact]
    public void IgnoresConditionsOutsideNestedHost ()
    {
        meta.Update(new() {
            Commands = [
                CreateConditionalCommand("if", "c"),
                new() { Id = "do", Parameters = [CreateConditionParamMeta("c")] },
            ]
        });
        var ifLine1 = new CommandLine(new("if", [new("c", new[] { new Parsing.Expression("e1") })]));
        var ifLine2 = new CommandLine(new("if", [new("c", new[] { new Parsing.Expression("e2") })]));
        var doLine = new CommandLine(new("do"), 1);
        var script = new IScriptLine[] { ifLine1, ifLine2, doLine };
        var resolved = new List<Condition>();
        Assert.True(resolver.TryResolve(doLine.Command, script, resolved));
        Assert.Equal([
            new(ifLine2, ifLine2.Command, ifLine2.Command.Parameters[0])
        ], resolved);
    }

    [Fact]
    public void IgnoresUnrelatedLines ()
    {
        meta.Update(new() {
            Commands = [
                CreateConditionalCommand("if", "c"),
                new() { Id = "do", Parameters = [CreateConditionParamMeta("c")] }
            ]
        });
        var self = new Parsing.Command("do");
        var script = new IScriptLine[] {
            new CommandLine(new("if", [new("c", new[] { new Parsing.Expression("x") })])),
            new CommandLine(new("if", [new("c", new[] { new Parsing.Expression("e1") })])),
            new CommentLine("foo", 1),
            new LabelLine("bar", 1),
            new CommandLine(new("c"), 1),
            new GenericLine([new InlinedCommand(new("if", [new("c", new[] { new Parsing.Expression("x") })]))], 1),
            new CommandLine(new("if", [new("c", new[] { new Parsing.Expression("x") })]), 1),
            new CommandLine(new("c"), 2),
            new CommandLine(new("if", [new("c", new[] { new Parsing.Expression("e2") })]), 1),
            new CommandLine(new("c"), 2),
            new GenericLine([new MixedValue([new PlainText("x")]), new InlinedCommand(self), new MixedValue([new PlainText("x")])], 2),
            new CommandLine(new("if", [new("c", new[] { new Parsing.Expression("x") })]), 2),
            new CommandLine(new("if", [new("c", new[] { new Parsing.Expression("x") })]), 1),
            new CommandLine(new("if", [new("c", new[] { new Parsing.Expression("x") })]))
        };
        var resolved = new List<Condition>();
        Assert.True(resolver.TryResolve(self, script, resolved));
        Assert.Equal(2, resolved.Count);
        Assert.Equal("{e2}", resolved[0].Parameter!.Value.ToString());
        Assert.Equal("{e1}", resolved[1].Parameter!.Value.ToString());
    }

    [Fact]
    public void WhenHostHasConditionParameterButNotNestedBranchFlagItsNotAdded ()
    {
        meta.Update(new() {
            Commands = [
                new() { Id = "x", Parameters = [CreateConditionParamMeta("c")], Nest = new(), Branch = new() { Traits = BranchTraits.Endpoint } },
                new() { Id = "do" }
            ]
        });
        var self = new Parsing.Command("do");
        var script = new IScriptLine[] {
            new CommandLine(new("x", [new("c", new[] { new Parsing.Expression("e1") })])),
            new CommandLine(self, 1)
        };
        var resolved = new List<Condition>();
        Assert.False(resolver.TryResolve(self, script, resolved));
        Assert.Empty(resolved);
    }

    [Fact]
    public void WhenHostHasNoConditionParameterAssignedItsNotAdded ()
    {
        meta.Update(new() {
            Commands = [
                CreateConditionalCommand("else", "c"),
                new() { Id = "do" }
            ]
        });
        var self = new Parsing.Command("do");
        var script = new IScriptLine[] {
            new CommandLine(new("else")),
            new CommandLine(self, 1)
        };
        var resolved = new List<Condition>();
        Assert.False(resolver.TryResolve(self, script, resolved));
        Assert.Empty(resolved);
    }

    [Fact]
    public void WhenHostHasNoConditionButHasInteractiveBranchFlagItsAdded ()
    {
        meta.Update(new() {
            Commands = [
                new() { Id = "choice", Nest = new(), Branch = new() { Traits = BranchTraits.Nest | BranchTraits.Interactive } },
                new() { Id = "do" }
            ]
        });
        var self = new Parsing.Command("do");
        var choice = new Parsing.Command("choice");
        var script = new IScriptLine[] {
            new CommandLine(choice),
            new CommandLine(self, 1)
        };
        var resolved = new List<Condition>();
        Assert.True(resolver.TryResolve(self, script, resolved));
        Assert.Equal([
            new(script[0], choice)
        ], resolved);
    }

    [Fact]
    public void SwitchConditionsAreNegated ()
    {
        meta.Update(new() {
            Commands = [
                CreateConditionalCommand("if", "c"),
                CreateConditionalCommand("else", "c", "if"),
                new() { Id = "do" }
            ]
        });
        var self = new Parsing.Command("do");
        var @if = new Parsing.Command("if", [new("c", new[] { new Parsing.Expression("e1") })]);
        var else1 = new Parsing.Command("else", [new("c", new[] { new Parsing.Expression("e2") })]);
        var else2 = new Parsing.Command("else", [new("c", new[] { new Parsing.Expression("e3") })]);
        var script = new IScriptLine[] {
            new CommandLine(@if),
            new CommandLine(new("x"), 1),
            new CommandLine(else1),
            new CommandLine(new("x"), 1),
            new CommandLine(else2),
            new CommandLine(self, 1),
        };
        var resolved = new List<Condition>();
        Assert.True(resolver.TryResolve(self, script, resolved));
        Assert.Equal([
            new(script[4], else2, else2.Parameters[0]),
            new(script[2], else1, else1.Parameters[0], true),
            new(script[0], @if, @if.Parameters[0], true)
        ], resolved);
    }

    [Fact]
    public void UnrelatedSwitchesAreNotLeaked ()
    {
        meta.Update(new() {
            Commands = [
                CreateConditionalCommand("if", "c"),
                CreateConditionalCommand("else", "c", "if"),
                new() { Id = "do" }
            ]
        });
        var self = new Parsing.Command("do");
        var if1 = new Parsing.Command("if", [new("c", new[] { new Parsing.Expression("x") })]);
        var if2 = new Parsing.Command("if", [new("c", new[] { new Parsing.Expression("e") })]);
        var @else = new Parsing.Command("else", [new("c", new[] { new Parsing.Expression("x") })]);
        var script = new IScriptLine[] {
            new CommandLine(if1),
            new CommandLine(new("x"), 1),
            new CommandLine(@else),
            new CommandLine(new("x"), 1),
            new CommandLine(if2),
            new CommandLine(self, 1),
        };
        var resolved = new List<Condition>();
        Assert.True(resolver.TryResolve(self, script, resolved));
        Assert.Equal([
            new(script[4], if2, if2.Parameters[0])
        ], resolved);
    }

    [Fact]
    public void NestedSwitchesResolvedCorrectly ()
    {
        meta.Update(new() {
            Commands = [
                CreateConditionalCommand("if", "c"),
                CreateConditionalCommand("else", "c", "if"),
                new() { Id = "do", Parameters = [CreateConditionParamMeta("c")] }
            ]
        });
        var self = new Parsing.Command("do", [new("c", new[] { new Parsing.Expression("e2") })]);
        var script = new IScriptLine[] {
            new CommandLine(new("if", [new("c", new[] { new Parsing.Expression("ne1") })])),
            new CommandLine(new("do"), 1),
            new CommandLine(new("else", [new("c", new[] { new Parsing.Expression("ne2") })])),
            new CommandLine(new("do"), 1),
            new CommandLine(new("else")),
            new CommandLine(new("if", [new("c", new[] { new Parsing.Expression("x") })]), 1),
            new CommandLine(new("do"), 2),
            new CommandLine(new("if", [new("c", new[] { new Parsing.Expression("ne3") })]), 1),
            new CommandLine(new("do"), 2),
            new CommandLine(new("else", [new("c", new[] { new Parsing.Expression("e1") })]), 1),
            new CommandLine(self, 2)
        };
        var resolved = new List<Condition>();
        Assert.True(resolver.TryResolve(self, script, resolved));
        Assert.Equal(5, resolved.Count);
        Assert.True(resolved[0].Parameter!.Value.ToString() == "{e2}" && !resolved[0].Inverted);
        Assert.True(resolved[1].Parameter!.Value.ToString() == "{e1}" && !resolved[1].Inverted);
        Assert.True(resolved[2].Parameter!.Value.ToString() == "{ne3}" && resolved[2].Inverted);
        Assert.True(resolved[3].Parameter!.Value.ToString() == "{ne2}" && resolved[3].Inverted);
        Assert.True(resolved[4].Parameter!.Value.ToString() == "{ne1}" && resolved[4].Inverted);
    }

    private Parameter CreateConditionParamMeta (string id) => new() {
        Id = id,
        ValueType = ValueType.String,
        ValueContext = [new() { Type = ValueContextType.Expression, SubType = Constants.Condition }]
    };

    private Command CreateConditionalCommand (string commandId, string conditionParamId, string switchRoot = null) => new() {
        Id = commandId,
        Parameters = [CreateConditionParamMeta(conditionParamId)],
        Nest = new(),
        Branch = new() {
            Traits = switchRoot is null ? (BranchTraits.Nest | BranchTraits.Switch) : BranchTraits.Nest,
            SwitchRoot = switchRoot
        }
    };
}
