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
        Assert.Null(new TokenMapper().Resolve(component));
    }

    [Fact]
    public void MapperMapsTokenOnAssociation ()
    {
        var component = new Mock<ILineComponent>().Object;
        var token = new Token();
        var mapper = new TokenMapper();
        mapper.Associate(component, token);
        Assert.Equal(token, mapper.Resolve(component));
    }

    [Fact]
    public void WhenClearedMapperLosesAssociations ()
    {
        var component = new Mock<ILineComponent>().Object;
        var token = new Token();
        var mapper = new TokenMapper();
        mapper.Associate(component, token);
        mapper.Clear();
        Assert.Null(mapper.Resolve(component));
    }
}
