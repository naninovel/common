using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Naninovel.Parsing.Utilities;

namespace Naninovel.Parsing;

/// <summary>
/// Allows transforming parsed script line semantic models back to text form.
/// </summary>
public class ScriptSerializer
{
    private readonly StringBuilder builder = new();
    private readonly StringBuilder mixedBuilder = new();
    private readonly List<(int, int)> ignoreRanges = new();

    /// <summary>
    /// Transforms provided script line semantic model back to text form.
    /// </summary>
    public string Serialize (IScriptLine line)
    {
        builder.Clear();
        AppendLine(line);
        return builder.ToString();
    }

    /// <summary>
    /// Transforms provided script line semantic models back to text form.
    /// </summary>
    public string Serialize (IEnumerable<IScriptLine> lines)
    {
        builder.Clear();
        foreach (var line in lines)
        {
            AppendLine(line);
            builder.Append('\n');
        }
        return TrimTrailingLineBreaks(builder).ToString();
    }

    /// <summary>
    /// Transforms provided mixed value semantic model back to text form.
    /// </summary>
    /// <param name="value">The value to transform.</param>
    /// <param name="wrap">Whether to wrap in quotes when whitespace is detected in plain text.</param>
    public string Serialize (IEnumerable<IValueComponent> value, bool wrap)
    {
        builder.Clear();
        AppendMixed(value, wrap);
        return builder.ToString();
    }

    private void AppendLine (IScriptLine line)
    {
        if (line is CommentLine comment) AppendCommentLine(comment);
        if (line is LabelLine label) AppendLabelLine(label);
        if (line is CommandLine command) AppendCommandLine(command);
        if (line is GenericLine generic) AppendGenericLine(generic);
    }

    private void AppendCommentLine (CommentLine commentLine)
    {
        builder.Append(Identifiers.CommentLine[0]);
        builder.Append(' ');
        builder.Append(commentLine.Comment);
    }

    private void AppendLabelLine (LabelLine labelLine)
    {
        builder.Append(Identifiers.LabelLine[0]);
        builder.Append(' ');
        builder.Append(labelLine.Label);
    }

    private void AppendCommandLine (CommandLine commandLine)
    {
        builder.Append(Identifiers.CommandLine[0]);
        AppendCommand(commandLine.Command);
    }

    private void AppendGenericLine (GenericLine genericLine)
    {
        if (genericLine.Prefix != null)
            AppendGenericPrefix(genericLine.Prefix);
        foreach (var content in genericLine.Content)
            if (content is MixedValue text) AppendMixed(text, false);
            else AppendInlinedCommand((InlinedCommand)content);
    }

    private void AppendCommand (Command command)
    {
        builder.Append(command.Identifier);
        var nameless = command.Parameters.FirstOrDefault(p => p.Nameless);
        if (nameless != null) AppendParameter(nameless);
        foreach (var param in command.Parameters)
            if (!param.Nameless)
                AppendParameter(param);
    }

    private void AppendParameter (Parameter parameter)
    {
        builder.Append(' ');

        if (!parameter.Nameless)
        {
            builder.Append(parameter.Identifier!);
            builder.Append(Identifiers.ParameterAssign[0]);
        }

        AppendMixed(parameter.Value, true);
    }

    private void AppendGenericPrefix (GenericPrefix prefix)
    {
        builder.Append(prefix.Author);
        if (prefix.Appearance != null)
        {
            builder.Append(Identifiers.AuthorAppearance[0]);
            builder.Append(prefix.Appearance);
        }
        builder.Append(Identifiers.AuthorAssign);
    }

    private void AppendInlinedCommand (InlinedCommand inlined)
    {
        builder.Append(Identifiers.InlinedOpen[0]);
        AppendCommand(inlined.Command);
        builder.Append(Identifiers.InlinedClose[0]);
    }

    private void AppendMixed (IEnumerable<IValueComponent> mixed, bool wrap)
    {
        ignoreRanges.Clear();
        mixedBuilder.Clear();
        foreach (var component in mixed)
            AppendComponent(component);
        builder.Append(Encode(mixedBuilder.ToString(), wrap));
    }

    private void AppendComponent (IValueComponent component)
    {
        if (component is PlainText text) mixedBuilder.Append(text);
        else if (component is Expression expression)
        {
            var startIndex = mixedBuilder.Length;
            mixedBuilder.Append(Identifiers.ExpressionOpen);
            mixedBuilder.Append(expression.Body);
            mixedBuilder.Append(Identifiers.ExpressionClose);
            ignoreRanges.Add((startIndex, mixedBuilder.Length - startIndex));
        }
    }

    private string Encode (string value, bool wrap = true)
    {
        wrap = wrap && (value[0] == '\"' || IsAnySpaceOrUnclosedQuotes());
        for (int i = value.Length - 1; i >= 0; i--)
            if (ShouldEscape(i))
                value = value.Insert(i, "\\");
        if (wrap) value = $"\"{value}\"";
        return value;

        bool IsAnySpaceOrUnclosedQuotes ()
        {
            var wrapping = false;
            for (int i = 0; i < value.Length; i++)
                if (IsIgnored(ignoreRanges, i)) continue;
                else if (char.IsWhiteSpace(value[i])) return true;
                else if (value[i] == '"' && !IsEscaped(value, i)) wrapping = !wrapping;
            return wrapping;
        }

        bool ShouldEscape (int i)
        {
            if (IsIgnored(ignoreRanges, i)) return false;
            return IsPlainTextControlChar(value[i]) || wrap && value[i] == '"';
        }

        static bool IsIgnored (IEnumerable<(int start, int length)> ignoredRanges, int i)
        {
            foreach (var (start, length) in ignoredRanges)
                if (i >= start && i < start + length)
                    return true;
            return false;
        }
    }

    public static StringBuilder TrimTrailingLineBreaks (StringBuilder builder)
    {
        var trimIndex = builder.Length - 1;
        for (; trimIndex >= 0; trimIndex--)
            if (builder[trimIndex] != '\n')
                break;
        trimIndex++;
        if (trimIndex < builder.Length - 1)
            builder.Length = trimIndex + 1;
        return builder;
    }
}
