using Xunit;
using static Naninovel.Metadata.ValueContainerType;
using static Naninovel.Metadata.ValueType;

namespace Naninovel.Metadata.Test;

public class ValueValidatorTest
{
    private readonly ValueValidator validator = new();

    [Theory]
    [InlineData(null, Single, String, true)]
    [InlineData(null, List, Boolean, true)]
    [InlineData(null, Named, Decimal, true)]
    [InlineData(null, NamedList, Integer, true)]
    [InlineData(" ", Single, String, true)]
    [InlineData(" ", List, Boolean, false)]
    [InlineData(" ", Named, Decimal, false)]
    [InlineData(" ", NamedList, Integer, false)]
    [InlineData(",,", List, Boolean, true)]
    [InlineData(".", Named, Decimal, true)]
    [InlineData(".,,.", NamedList, Integer, true)]
    [InlineData("foo", Single, String, true)]
    [InlineData("0", Single, Integer, true)]
    [InlineData("-0101", Single, Integer, true)]
    [InlineData("0.1", Single, Integer, false)]
    [InlineData("1f", Single, Integer, false)]
    [InlineData("00.1", Single, Decimal, true)]
    [InlineData("-01", Single, Decimal, true)]
    [InlineData("1f", Single, Decimal, false)]
    [InlineData("true", Single, Boolean, true)]
    [InlineData("false", Single, Boolean, true)]
    [InlineData("True", Single, Boolean, true)]
    [InlineData("False", Single, Boolean, true)]
    [InlineData("+", Single, Boolean, false)]
    [InlineData("yes", Single, Boolean, false)]
    [InlineData("1,2,3", List, Integer, true)]
    [InlineData("1,0.2,3", List, Integer, false)]
    [InlineData(".,foo.0.2,bar", NamedList, Decimal, true)]
    public void ValidationTheory (string value, ValueContainerType container, ValueType type, bool expected)
    {
        Assert.Equal(expected, validator.Validate(value, container, type));
    }
}
