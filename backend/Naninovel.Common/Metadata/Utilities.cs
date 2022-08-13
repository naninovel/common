using System;
using System.Text;
using ContainerType = Naninovel.Metadata.ValueContainerType;

namespace Naninovel.Metadata;

internal static class Utilities
{
    public static string BuildLabel (string? alias, string id)
    {
        if (!string.IsNullOrEmpty(alias)) return ToFirstLower(alias!);
        return ToFirstLower(id);
    }

    public static string BuildTypeLabel (ValueType valueType, ContainerType containerType)
    {
        var builder = new StringBuilder();
        if (containerType is ContainerType.Named or ContainerType.NamedList)
            builder.Append("named ");
        builder.Append(ToFirstLower(Enum.GetName(typeof(ValueType), valueType)!));
        if (containerType is ContainerType.List or ContainerType.NamedList)
            builder.Append(" list");
        return builder.ToString();
    }

    private static string ToFirstLower (string value)
    {
        if (value.Length == 1) char.ToLowerInvariant(value[0]);
        return char.ToLowerInvariant(value[0]) + value.Substring(1);
    }
}
