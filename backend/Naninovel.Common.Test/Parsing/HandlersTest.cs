using System.Collections;
using System.Collections.Generic;
using Moq;
using Xunit;

namespace Naninovel.Parsing.Test;

public class HandlersTest
{
    [Fact]
    public void ErrorCollectorAddsError ()
    {
        var collector = new ErrorCollector();
        var error = new ParseError("", 0, 0);
        collector.HandleError(error);
        Assert.Contains(error, collector);
    }

    [Fact]
    public void WhenNotFoundMapperResolvesToFalse ()
    {
        var component = new Mock<ILineComponent>().Object;
        Assert.False(new RangeMapper().TryResolve(component, out _));
    }

    [Fact]
    public void MapperMapsRangeOnAssociation ()
    {
        var component = new Mock<ILineComponent>().Object;
        var range = new LineRange();
        var mapper = new RangeMapper();
        mapper.Associate(component, range);
        Assert.True(mapper.TryResolve(component, out var result));
        Assert.Equal(range, result);
    }

    [Fact]
    public void WhenClearedMapperLosesAssociations ()
    {
        var component = new Mock<ILineComponent>().Object;
        var range = new LineRange();
        var mapper = new RangeMapper();
        mapper.Associate(component, range);
        mapper.Clear();
        Assert.False(mapper.TryResolve(component, out _));
    }

    [Fact]
    public void CanEnumerateOverMapper ()
    {
        var component = new Mock<ILineComponent>().Object;
        var range = new LineRange();
        var mapper = new RangeMapper();
        mapper.Associate(component, range);

        var enumerator = ((IEnumerable)mapper).GetEnumerator();
        enumerator.MoveNext();
        Assert.Equal(component, ((KeyValuePair<ILineComponent, LineRange>)enumerator.Current!).Key);
        Assert.Equal(range, ((KeyValuePair<ILineComponent, LineRange>)enumerator.Current!).Value);

        foreach (var kv in mapper)
        {
            Assert.Equal(component, kv.Key);
            Assert.Equal(range, kv.Value);
        }
    }

    [Fact]
    public void WhenNothingIdentifiedMapperIsEmpty ()
    {
        Assert.Empty(new TextMapper().Map);
    }

    [Fact]
    public void TextMapperMapsIdentifiedText ()
    {
        var mapper = new TextMapper();
        mapper.Identify("f", "foo");
        Assert.Equal("foo", mapper.Map["f"]);
    }

    [Fact]
    public void WhenClearedMapperIsEmpty ()
    {
        var mapper = new TextMapper();
        mapper.Identify("f", "foo");
        mapper.Clear();
        Assert.Empty(mapper.Map);
    }
}
