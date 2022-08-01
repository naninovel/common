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
    public void WhenNotFoundMapperResolvesToNull ()
    {
        var component = new Mock<ILineComponent>().Object;
        Assert.Null(new RangeMapper().Resolve(component));
    }

    [Fact]
    public void MapperMapsRangeOnAssociation ()
    {
        var component = new Mock<ILineComponent>().Object;
        var range = new LineRange();
        var mapper = new RangeMapper();
        mapper.Associate(component, range);
        Assert.Equal(range, mapper.Resolve(component));
    }

    [Fact]
    public void WhenClearedMapperLosesAssociations ()
    {
        var component = new Mock<ILineComponent>().Object;
        var range = new LineRange();
        var mapper = new RangeMapper();
        mapper.Associate(component, range);
        mapper.Clear();
        Assert.Null(mapper.Resolve(component));
    }
}
