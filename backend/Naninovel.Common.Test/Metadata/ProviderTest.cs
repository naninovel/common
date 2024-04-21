namespace Naninovel.Metadata.Test;

public class ProviderTest
{
    private readonly MetadataProvider provider = new();

    [Fact]
    public void ProjectMetadataIsAssignedToCollections ()
    {
        provider.Update(new() {
            Actors = [new Actor { Id = "foo" }],
            Constants = [new Constant { Name = "bar" }],
            Resources = [new Resource { Path = "nya" }]
        });
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
        var meta = new Project { Commands = [new Command { Id = "foo" }] };
        provider.Update(meta);
        Assert.Equal(meta.Commands[0], provider.FindCommand("foo"));
    }

    [Fact]
    public void CanFindCommandByAlias ()
    {
        var meta = new Project { Commands = [new Command { Id = "foo", Alias = "f" }] };
        provider.Update(meta);
        Assert.Equal(meta.Commands[0], provider.FindCommand("f"));
    }

    [Fact]
    public void CanFindParameterById ()
    {
        var param = new Parameter { Id = "bar" };
        var meta = new Project { Commands = [new Command { Id = "foo", Parameters = [param] }] };
        provider.Update(meta);
        Assert.Equal(param, provider.FindParameter("foo", "bar"));
    }

    [Fact]
    public void CanFindParameterByAlias ()
    {
        var param = new Parameter { Id = "bar", Alias = "b" };
        var meta = new Project { Commands = [new Command { Id = "foo", Parameters = [param] }] };
        provider.Update(meta);
        Assert.Equal(param, provider.FindParameter("foo", "b"));
    }

    [Fact]
    public void CanFindNamelessParameter ()
    {
        var param = new Parameter { Id = "bar", Nameless = true };
        var meta = new Project { Commands = [new Command { Id = "foo", Parameters = [param] }] };
        provider.Update(meta);
        Assert.Equal(param, provider.FindParameter("foo", ""));
    }

    [Fact]
    public void CanUseNullToRepresentNamelessParameter ()
    {
        var param = new Parameter { Id = "bar", Nameless = true };
        var meta = new Project { Commands = [new Command { Id = "foo", Parameters = [param] }] };
        provider.Update(meta);
        Assert.Equal(param, provider.FindParameter("foo", null));
    }

    [Fact]
    public void WhenMetaHasNoAliasButIsNotNamelessItsNotConsideredNameless ()
    {
        var param1 = new Parameter { Id = "1" };
        var param2 = new Parameter { Id = "2", Alias = "" };
        var meta = new Project { Commands = [new Command { Id = "cmd", Parameters = [param1, param2] }] };
        provider.Update(meta);
        Assert.Null(provider.FindParameter("cmd", null));
        Assert.Null(provider.FindParameter("cmd", ""));
    }

    [Fact]
    public void CanCreateProviderWithMeta ()
    {
        var provider = new MetadataProvider(new() {
            Variables = ["foo"],
            Functions = ["bar"]
        });
        Assert.Equal("foo", provider.Variables.First());
        Assert.Equal("bar", provider.Functions.First());
    }

    [Fact]
    public void PreferencesAreCopied ()
    {
        var prefs = new Preferences();
        var provider = new MetadataProvider(new Project { Preferences = prefs });
        Assert.NotSame(prefs, provider.Preferences);
    }
}
