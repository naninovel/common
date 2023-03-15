using Naninovel.Parsing;
using Xunit;

namespace Naninovel.Metadata.Test;

public class EndpointResolverTest
{
    private readonly EndpointResolver resolver;
    private readonly MetadataProvider meta = new();

    public EndpointResolverTest ()
    {
        resolver = new(meta);
    }

    [Fact]
    public void WhenUnknownCommandReturnsFalse ()
    {
        Assert.False(resolver.TryResolve(new("c"), out _, out _));
    }

    [Fact]
    public void WhenUnknownParameterReturnsFalse ()
    {
        meta.Update(new() {
            Commands = new[] { new Command { Id = "c" } }
        });
        Assert.False(resolver.TryResolve(new("c"), out _, out _));
    }

    [Fact]
    public void WhenOtherParameterReturnsFalse ()
    {
        meta.Update(new() {
            Commands = new[] { new Command { Id = "c", Parameters = new[] { new Parameter { Id = "p" } } } }
        });
        Assert.False(resolver.TryResolve(new("c", new[] { new Parsing.Parameter("p", new[] { new PlainText("v") }) }), out _, out _));
    }

    [Fact]
    public void CanResolveScript ()
    {
        meta.Update(new() {
            Commands = new[] { new Command { Id = "c", Parameters = new[] { CreateEndpointParameterMeta("p") } } }
        });
        Assert.True(resolver.TryResolve(new("c", new[] { new Parsing.Parameter("p", new[] { new PlainText("s") }) }), out var script, out _));
        Assert.Equal("s", script);
    }

    [Fact]
    public void CanResolveLabel ()
    {
        meta.Update(new() {
            Commands = new[] { new Command { Id = "c", Parameters = new[] { CreateEndpointParameterMeta("p") } } }
        });
        Assert.True(resolver.TryResolve(new("c", new[] { new Parsing.Parameter("p", new[] { new PlainText(".l") }) }), out _, out var label));
        Assert.Equal("l", label);
    }

    [Fact]
    public void CanResolveScriptAndLabel ()
    {
        meta.Update(new() {
            Commands = new[] { new Command { Id = "c", Parameters = new[] { CreateEndpointParameterMeta("p") } } }
        });
        Assert.True(resolver.TryResolve(new("c", new[] { new Parsing.Parameter("p", new[] { new PlainText("s.l") }) }), out var script, out var label));
        Assert.Equal("s", script);
        Assert.Equal("l", label);
    }

    private Parameter CreateEndpointParameterMeta (string id)
    {
        return new Parameter {
            Id = id,
            ValueType = ValueType.String,
            ValueContainerType = ValueContainerType.Named,
            ValueContext = new ValueContext[] {
                new() { Type = ValueContextType.Resource, SubType = "Scripts" },
                new() { Type = ValueContextType.Constant, SubType = "Labels/{:Path[0]??$Script}" }
            }
        };
    }
}
