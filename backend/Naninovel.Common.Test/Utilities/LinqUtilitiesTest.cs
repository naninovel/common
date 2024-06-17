using System.Diagnostics.CodeAnalysis;

namespace Naninovel.Utilities.Test;

[ExcludeFromCodeCoverage]
public class LinqUtilitiesTest
{
    [Fact]
    public void OrNullExtensionsReturnNullWhenElementNotFound ()
    {
        Assert.Null(new List<int>().FirstOrNull());
        Assert.Null(new Dictionary<int, int>().FirstOrNull());
        Assert.Null(new List<int>().FirstOrNull(x => false));
        Assert.Null(new Dictionary<int, int>().FirstOrNull(x => false));
        Assert.Null(new List<int>().LastOrNull());
        Assert.Null(new Dictionary<int, int>().LastOrNull());
        Assert.Null(new List<int>().LastOrNull(x => false));
        Assert.Null(new Dictionary<int, int>().LastOrNull(x => false));
        Assert.Null(new List<int>().AtOrNull(0));
        Assert.Null(new Dictionary<int, int>().AtOrNull(0));
    }

    [Fact]
    public void OrNullExtensionsReturnElementWhenFound ()
    {
        Assert.Equal(1, new List<int> { 1, 2 }.FirstOrNull());
        Assert.True(new Dictionary<int, int> { [1] = 1, [2] = 2 }.FirstOrNull() is { Key: 1, Value: 1 });
        Assert.Equal(2, new List<int> { 1, 2 }.FirstOrNull(x => x > 1));
        Assert.True(new Dictionary<int, int> { [1] = 1, [2] = 2 }.FirstOrNull(x => x.Key > 1) is { Key: 2, Value: 2 });
        Assert.Equal(2, new List<int> { 1, 2 }.LastOrNull());
        Assert.True(new Dictionary<int, int> { [1] = 1, [2] = 2 }.LastOrNull() is { Key: 2, Value: 2 });
        Assert.Equal(1, new List<int> { 1, 2 }.LastOrNull(x => x < 2));
        Assert.True(new Dictionary<int, int> { [1] = 1, [2] = 2 }.LastOrNull(x => x.Key < 2) is { Key: 1, Value: 1 });
        Assert.Equal(1, new List<int> { 1, 2 }.AtOrNull(0));
        Assert.True(new Dictionary<int, int> { [1] = 1, [2] = 2 }.AtOrNull(1) is { Key: 2, Value: 2 });
    }

    [Fact]
    public void AtOrNullReturnsNullWhenOutOfRange ()
    {
        Assert.Null(new List<int> { 1, 2 }.AtOrNull(-1));
        Assert.Null(new List<int> { 1, 2 }.AtOrNull(2));
    }
}
