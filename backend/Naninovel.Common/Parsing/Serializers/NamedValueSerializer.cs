namespace Naninovel.Parsing;

/// <summary>
/// Allows transforming named values parsed via
/// <see cref="NamedValueParser"/> back to semantic model.
/// </summary>
public class NamedValueSerializer (ISyntax stx)
{
    private readonly Utilities utils = new(stx);

    public string Serialize (string? name, string? value)
    {
        if (name is null && value is null) return "";
        if (name is null) return $"{stx.NamedDelimiter}{value}";
        if (value is null) return EscapeName(name);
        return $"{EscapeName(name)}{stx.NamedDelimiter}{value}";
    }

    private string EscapeName (string name)
    {
        for (int i = name.Length - 1; i >= 0; i--)
            if (name[i] == stx.NamedDelimiter[0] && !utils.IsEscaped(name, i))
                name = name.Insert(i, "\\");
        return name;
    }
}
