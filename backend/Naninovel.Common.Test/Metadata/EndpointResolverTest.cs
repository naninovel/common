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
    public void WhenParameterHasNoEndpointContextReturnsFalse ()
    {
        meta.Update(new() {
            Commands = [new Command { Id = "c", Parameters = [new() { Id = "p" }] }]
        });
        Assert.False(resolver.TryResolve(new("c", [new("p", new[] { new PlainText("v") })]), out _));
    }

    [Fact]
    public void WhenCommandDoesntBranchReturnsFalse ()
    {
        meta.Update(new() {
            Commands = [new Command { Id = "c", Branch = null, Parameters = [CreateEndpointParamMeta("p")] }]
        });
        Assert.False(resolver.TryResolve(new("c", [new("p", new[] { new PlainText("v") })]), out _));
    }

    [Fact]
    public void WhenCommandDoesntHaveEndpointBranchFlagReturnsFalse ()
    {
        meta.Update(new() {
            Commands = [new Command { Id = "c", Branch = new() { Traits = BranchTraits.Nest }, Parameters = [CreateEndpointParamMeta("p")] }]
        });
        Assert.False(resolver.TryResolve(new("c", [new("p", new[] { new PlainText("v") })]), out _));
    }

    [Fact]
    public void WhenUnknownParameterReturnsFalse ()
    {
        meta.Update(new() {
            Commands = [new Command { Id = "c", Branch = new() { Traits = BranchTraits.Endpoint }, Parameters = [CreateEndpointParamMeta("p")] }]
        });
        Assert.False(resolver.TryResolve(new("x", new[] { new PlainText("v") }), "c", out _));
    }

    [Fact]
    public void WhenParameterIsNotAssignedReturnsFalse ()
    {
        meta.Update(new() {
            Commands = [CreateEndpointCommandMeta("c", "p")]
        });
        Assert.False(resolver.TryResolve(new("c"), out _));
    }

    [Fact]
    public void CanResolveScriptComponentOfEndpoint ()
    {
        meta.Update(new() {
            Commands = [CreateEndpointCommandMeta("c", "p")]
        });
        Assert.True(resolver.TryResolve(new("c", [new("p", new[] { new PlainText("s") })]), out var point));
        Assert.Equal("s", point.ScriptPath);
    }

    [Fact]
    public void CanResolveLabelComponentOfTheEndpoint ()
    {
        meta.Update(new() {
            Commands = [CreateEndpointCommandMeta("c", "p")]
        });
        Assert.True(resolver.TryResolve(new("c", [new("p", new[] { new PlainText(".l") })]), out var point));
        Assert.Equal("l", point.Label);
    }

    [Fact]
    public void CanResolveBothComponentsOfTheEndpoint ()
    {
        meta.Update(new() {
            Commands = [CreateEndpointCommandMeta("c", "p")]
        });
        Assert.True(resolver.TryResolve(new("c", [new("p", new[] { new PlainText("s.l") })]), out var point));
        Assert.Equal("s", point.ScriptPath);
        Assert.Equal("l", point.Label);
    }

    [Fact]
    public void CanResolveEndpointFromBranch ()
    {
        meta.Update(new() {
            Commands = [new() { Id = "c", Branch = new() { Traits = BranchTraits.Endpoint, Endpoint = "s" } }]
        });
        Assert.True(resolver.TryResolve(new("c"), out var point));
        Assert.Equal("s", point.ScriptPath);
    }

    [Fact]
    public void CanResolveEndpointFromBranchExpression ()
    {
        meta.Update(new() {
            TitleScript = "s",
            Commands = [new() { Id = "c", Branch = new() { Traits = BranchTraits.Endpoint, Endpoint = $"{{{ExpressionEvaluator.TitleScript}}}" } }]
        });
        Assert.True(resolver.TryResolve(new("c"), out var point));
        Assert.Equal("s", point.ScriptPath);
    }

    [Fact]
    public void WhenExpressionNotResolvedEndpointNotResolved ()
    {
        meta.Update(new() {
            TitleScript = null,
            Commands = [new() { Id = "c", Branch = new() { Traits = BranchTraits.Endpoint, Endpoint = $"{{{ExpressionEvaluator.TitleScript}}}" } }]
        });
        Assert.False(resolver.TryResolve(new("c"), out _));
    }

    [Fact] // Parameter endpoint context is expected to be used instead of requesting parameters in the expression.
    public void DoesntResolveWhenExpressionRequestsParameterValue ()
    {
        meta.Update(new() {
            Commands = [new() { Id = "c", Branch = new() { Traits = BranchTraits.Endpoint, Endpoint = "{:foo}" } }]
        });
        Assert.False(resolver.TryResolve(new("c"), out _));
    }

    private Parameter CreateEndpointParamMeta (string id) => new() {
        Id = id,
        ValueType = ValueType.String,
        ValueContainerType = ValueContainerType.Named,
        ValueContext = [
            new() { Type = ValueContextType.Endpoint, SubType = Constants.EndpointScript },
            new() { Type = ValueContextType.Endpoint, SubType = Constants.EndpointLabel }
        ]
    };

    private Command CreateEndpointCommandMeta (string commandId, string paramId) => new() {
        Id = commandId,
        Branch = new() { Traits = BranchTraits.Endpoint },
        Parameters = [CreateEndpointParamMeta(paramId)]
    };
}
