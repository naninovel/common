using Naninovel.Parsing;

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
    public void EndpointHashingIsCorrect ()
    {
        Assert.True(new Endpoint(null, null).Equals(new(null, null)));
        Assert.True(new Endpoint("foo", "bar").Equals(new("foo", "bar")));
        Assert.False(new Endpoint(null, "foo").Equals(new(null, null)));
        Assert.False(new Endpoint("foo", null).Equals(new(null, null)));
        Assert.False(new Endpoint("foo", "bar").Equals(new("bar", "foo")));
        Assert.False(new Endpoint().Equals(null));
        Assert.Equal(new Endpoint().GetHashCode(), new Endpoint().GetHashCode());
        Assert.Equal(new Endpoint("foo", "bar").GetHashCode(), new Endpoint("foo", "bar").GetHashCode());
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
            Commands = [new Command { Id = "c" }]
        });
        Assert.False(resolver.TryResolve(new("c"), out _));
    }

    [Fact]
    public void WhenOtherParameterReturnsFalse ()
    {
        meta.Update(new() {
            Commands = [new Command { Id = "c", Parameters = [new Parameter { Id = "p" }] }]
        });
        Assert.False(resolver.TryResolve(new("c", new[] { new Parsing.Parameter("p", new[] { new PlainText("v") }) }), out _));
    }

    [Fact]
    public void CanResolveScript ()
    {
        meta.Update(new() {
            Commands = [new Command { Id = "c", Parameters = [CreateEndpointParameterMeta("p")] }]
        });
        Assert.True(resolver.TryResolve(new("c", new[] { new Parsing.Parameter("p", new[] { new PlainText("s") }) }), out var point));
        Assert.Equal("s", point.Script);
    }

    [Fact]
    public void CanResolveLabel ()
    {
        meta.Update(new() {
            Commands = [new Command { Id = "c", Parameters = [CreateEndpointParameterMeta("p")] }]
        });
        Assert.True(resolver.TryResolve(new("c", new[] { new Parsing.Parameter("p", new[] { new PlainText(".l") }) }), out var point));
        Assert.Equal("l", point.Label);
    }

    [Fact]
    public void CanResolveScriptAndLabel ()
    {
        meta.Update(new() {
            Commands = [new Command { Id = "c", Parameters = [CreateEndpointParameterMeta("p")] }]
        });
        Assert.True(resolver.TryResolve(new("c", new[] { new Parsing.Parameter("p", new[] { new PlainText("s.l") }) }), out var point));
        Assert.Equal("s", point.Script);
        Assert.Equal("l", point.Label);
    }

    private Parameter CreateEndpointParameterMeta (string id)
    {
        return new Parameter {
            Id = id,
            ValueType = ValueType.String,
            ValueContainerType = ValueContainerType.Named,
            ValueContext = [
                new() { Type = ValueContextType.Endpoint, SubType = Constants.EndpointScript },
                new() { Type = ValueContextType.Endpoint, SubType = Constants.EndpointLabel }
            ]
        };
    }
}
