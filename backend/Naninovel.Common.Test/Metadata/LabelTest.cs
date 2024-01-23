using static Naninovel.Metadata.ValueContainerType;

namespace Naninovel.Metadata.Test;

public class LabelTest
{
    private readonly Project meta = new();

    [Fact]
    public void LabelIsAliasWhenAvailable ()
    {
        Assert.Equal("a", new Command { Id = "aliased", Alias = "a" }.Label);
        Assert.Equal("a", new Parameter { Id = "aliased", Alias = "a" }.Label);
    }

    [Fact]
    public void LabelIsIdWhenAlisIsNotAvailable ()
    {
        Assert.Equal("aliased", new Command { Id = "aliased" }.Label);
        Assert.Equal("aliased", new Parameter { Id = "aliased" }.Label);
    }

    [Fact]
    public void FirstLabelCharacterIsLowerCase ()
    {
        Assert.Equal("foo", new Command { Id = "Foo" }.Label);
        Assert.Equal("foo", new Parameter { Id = "Foo" }.Label);
    }

    [Fact]
    public void TypeLabelHasCorrectFormat ()
    {
        Assert.Equal("boolean", new Parameter { ValueType = ValueType.Boolean }.TypeLabel);
        Assert.Equal("named decimal", new Parameter { ValueType = ValueType.Decimal, ValueContainerType = Named }.TypeLabel);
        Assert.Equal("integer list", new Parameter { ValueType = ValueType.Integer, ValueContainerType = List }.TypeLabel);
        Assert.Equal("named string list", new Parameter { ValueType = ValueType.String, ValueContainerType = NamedList }.TypeLabel);
    }
}
