using System.Collections;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace Naninovel.Utilities.Test;

[ExcludeFromCodeCoverage]
public class LinqUtilTest
{
    private class CustomEnumerable (IEnumerable<int> items) : IEnumerable<int>
    {
        public IEnumerator<int> GetEnumerator () => items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }

    [Fact, SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public void CanCheckIfIndexIsValid ()
    {
        var str = "0";
        var array = new[] { 0 };
        var list = new List<int> { 0 };
        var dict = new Dictionary<int, int> { [0] = 0 };
        var set = new HashSet<int> { 0 };
        var frozen = set.ToFrozenSet();
        var enumerable = new CustomEnumerable([0]);
        ICollection<int> iColl = [0];
        IReadOnlyCollection<int> iRoColl = [0];
        IList<int> iList = [0];
        IReadOnlyList<int> iRoList = [0];

        Assert.True(str.IsIndexValid(0));
        Assert.False(str.IsIndexValid(-1));
        Assert.False(str.IsIndexValid(1));

        Assert.True(array.IsIndexValid(0));
        Assert.False(array.IsIndexValid(-1));
        Assert.False(array.IsIndexValid(1));

        Assert.True(list.IsIndexValid(0));
        Assert.False(list.IsIndexValid(-1));
        Assert.False(list.IsIndexValid(1));

        Assert.True(dict.IsIndexValid(0));
        Assert.False(dict.IsIndexValid(-1));
        Assert.False(dict.IsIndexValid(1));

        Assert.True(set.IsIndexValid(0));
        Assert.False(set.IsIndexValid(-1));
        Assert.False(set.IsIndexValid(1));

        Assert.True(frozen.IsIndexValid(0));
        Assert.False(frozen.IsIndexValid(-1));
        Assert.False(frozen.IsIndexValid(1));

        Assert.True(enumerable.IsIndexValid(0));
        Assert.False(enumerable.IsIndexValid(-1));
        Assert.False(enumerable.IsIndexValid(1));

        Assert.True(iColl.IsIndexValid(0));
        Assert.False(iColl.IsIndexValid(-1));
        Assert.False(iColl.IsIndexValid(1));

        Assert.True(iRoColl.IsIndexValid(0));
        Assert.False(iRoColl.IsIndexValid(-1));
        Assert.False(iRoColl.IsIndexValid(1));

        Assert.True(iList.IsIndexValid(0));
        Assert.False(iList.IsIndexValid(-1));
        Assert.False(iList.IsIndexValid(1));

        Assert.True(iRoList.IsIndexValid(0));
        Assert.False(iRoList.IsIndexValid(-1));
        Assert.False(iRoList.IsIndexValid(1));
    }

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
