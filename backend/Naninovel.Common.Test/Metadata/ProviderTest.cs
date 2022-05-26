using System.Linq;
using Xunit;

namespace Naninovel.Metadata.Test;

public class ProviderTest
{
    private MetadataProvider provider => new(meta);
    private readonly Project meta = new();

    [Fact]
    public void ProjectMetadataIsAssignedToCollections ()
    {
        meta.Actors = new[] { new Actor { Id = "foo" } };
        meta.Constants = new[] { new Constant { Name = "bar" } };
        meta.Resources = new[] { new Resource { Path = "nya" } };
        Assert.Equal("foo", provider.Actors.First().Id);
        Assert.Equal("bar", provider.Constants.First().Name);
        Assert.Equal("nya", provider.Resources.First().Path);
    }

    [Fact]
    public void WhenCommandNotFoundNullIsReturned ()
    {
        Assert.Null(provider.FindCommand(""));
    }

    [Fact]
    public void WhenParameterNotFoundNullIsReturned ()
    {
        Assert.Null(provider.FindParameter("", ""));
    }

    [Fact]
    public void CanFindCommandById ()
    {
        meta.Commands = new[] { new Command { Id = "foo" } };
        Assert.Equal(meta.Commands[0], provider.FindCommand("foo"));
    }

    [Fact]
    public void CanFindCommandByAlias ()
    {
        meta.Commands = new[] { new Command { Id = "foo", Alias = "f" } };
        Assert.Equal(meta.Commands[0], provider.FindCommand("f"));
    }

    [Fact]
    public void CanFindParameterById ()
    {
        var param = new Parameter { Id = "bar" };
        meta.Commands = new[] { new Command { Id = "foo", Parameters = new[] { param } } };
        Assert.Equal(param, provider.FindParameter("foo", "bar"));
    }

    [Fact]
    public void CanFindParameterByAlias ()
    {
        var param = new Parameter { Id = "bar", Alias = "b" };
        meta.Commands = new[] { new Command { Id = "foo", Parameters = new[] { param } } };
        Assert.Equal(param, provider.FindParameter("foo", "b"));
    }

    [Fact]
    public void CanFindNamelessParameter ()
    {
        var param = new Parameter { Id = "bar", Nameless = true };
        meta.Commands = new[] { new Command { Id = "foo", Parameters = new[] { param } } };
        Assert.Equal(param, provider.FindParameter("foo", ""));
    }
}
