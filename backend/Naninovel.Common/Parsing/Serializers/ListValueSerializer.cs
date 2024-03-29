﻿using System.Text;
using static Naninovel.Parsing.Utilities;
using static Naninovel.Parsing.Identifiers;

namespace Naninovel.Parsing;

/// <summary>
/// Allows transforming list values parsed via
/// <see cref="ListValueParser"/> back to semantic model.
/// </summary>
public class ListValueSerializer
{
    private readonly StringBuilder builder = new();

    public string Serialize (IReadOnlyList<string?> value)
    {
        builder.Clear();
        for (int i = 0; i < value.Count; i++)
            AppendElement(value, i);
        return builder.ToString();
    }

    private void AppendElement (IReadOnlyList<string?> value, int index)
    {
        var element = value[index];
        if (index > 0) builder.Append(ListDelimiter[0]);
        if (element != null) builder.Append(EscapeElement(element));
    }

    private string EscapeElement (string element)
    {
        for (int i = element.Length - 1; i >= 0; i--)
            if (element[i] == ListDelimiter[0] && !IsEscaped(element, i))
                element = element.Insert(i, "\\");
        return element;
    }
}
