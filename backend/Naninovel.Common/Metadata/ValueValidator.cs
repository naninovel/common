using System.Globalization;
using Naninovel.Parsing;

namespace Naninovel.Metadata;

/// <summary>
/// Allows checking if parameter values fit associated metadata.
/// </summary>
public class ValueValidator
{
    private readonly ListValueParser listParser = new();
    private readonly NamedValueParser namedParser = new();
    private string value = null!;
    private ValueType type;

    /// <summary>
    /// Checks whether specified parameter value text fits specified metadata.
    /// </summary>
    public bool Validate (string? value, ValueContainerType container, ValueType type)
    {
        if (value == null) return true;
        Reset(value, type);
        if (container is ValueContainerType.List) return ValidateList();
        if (container is ValueContainerType.NamedList) return ValidateNamedList();
        if (container is ValueContainerType.Named) return ValidateNamed(value);
        return ValidateSingle(value);
    }

    private void Reset (string value, ValueType type)
    {
        this.value = value;
        this.type = type;
    }

    private bool ValidateList ()
    {
        foreach (var item in listParser.Parse(value))
            if (!ValidateSingle(item))
                return false;
        return true;
    }

    private bool ValidateNamedList ()
    {
        foreach (var item in listParser.Parse(value))
            if (item != null && !ValidateNamed(item))
                return false;
        return true;
    }

    private bool ValidateNamed (string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        var (_, namedValue) = namedParser.Parse(value);
        return ValidateSingle(namedValue);
    }

    private bool ValidateSingle (string? value)
    {
        if (value == null) return true;
        if (string.IsNullOrWhiteSpace(value)) return type == ValueType.String;
        if (type == ValueType.Integer) return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _);
        if (type == ValueType.Decimal) return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _);
        if (type == ValueType.Boolean) return bool.TryParse(value, out _);
        return true;
    }
}
