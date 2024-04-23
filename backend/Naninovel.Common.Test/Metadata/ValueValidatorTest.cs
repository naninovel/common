using Naninovel.Parsing;

namespace Naninovel.Metadata.Test;

public class ValueValidatorTest
{
    private readonly ValueValidator validator = new(Identifiers.Default);

    [Theory]
    [InlineData(null, ValueContainerType.Single, ValueType.String, true)]
    [InlineData(null, ValueContainerType.List, ValueType.Boolean, true)]
    [InlineData(null, ValueContainerType.Named, ValueType.Decimal, true)]
    [InlineData(null, ValueContainerType.NamedList, ValueType.Integer, true)]
    [InlineData(" ", ValueContainerType.Single, ValueType.String, true)]
    [InlineData(" ", ValueContainerType.List, ValueType.Boolean, false)]
    [InlineData(" ", ValueContainerType.Named, ValueType.Decimal, false)]
    [InlineData(" ", ValueContainerType.NamedList, ValueType.Integer, false)]
    [InlineData(",,", ValueContainerType.List, ValueType.Boolean, true)]
    [InlineData(".", ValueContainerType.Named, ValueType.Decimal, true)]
    [InlineData(".,,.", ValueContainerType.NamedList, ValueType.Integer, true)]
    [InlineData("foo", ValueContainerType.Single, ValueType.String, true)]
    [InlineData("0", ValueContainerType.Single, ValueType.Integer, true)]
    [InlineData("-0101", ValueContainerType.Single, ValueType.Integer, true)]
    [InlineData("0.1", ValueContainerType.Single, ValueType.Integer, false)]
    [InlineData("1f", ValueContainerType.Single, ValueType.Integer, false)]
    [InlineData("00.1", ValueContainerType.Single, ValueType.Decimal, true)]
    [InlineData("-01", ValueContainerType.Single, ValueType.Decimal, true)]
    [InlineData("1f", ValueContainerType.Single, ValueType.Decimal, false)]
    [InlineData("true", ValueContainerType.Single, ValueType.Boolean, true)]
    [InlineData("false", ValueContainerType.Single, ValueType.Boolean, true)]
    [InlineData("True", ValueContainerType.Single, ValueType.Boolean, true)]
    [InlineData("False", ValueContainerType.Single, ValueType.Boolean, true)]
    [InlineData("+", ValueContainerType.Single, ValueType.Boolean, false)]
    [InlineData("yes", ValueContainerType.Single, ValueType.Boolean, false)]
    [InlineData("1,2,3", ValueContainerType.List, ValueType.Integer, true)]
    [InlineData("1,0.2,3", ValueContainerType.List, ValueType.Integer, false)]
    [InlineData(".,foo.0.2,bar", ValueContainerType.NamedList, ValueType.Decimal, true)]
    public void ValidationTheory (string value, ValueContainerType container, ValueType type, bool expected)
    {
        Assert.Equal(expected, validator.Validate(value, container, type));
    }
}
