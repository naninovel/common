using static Naninovel.Parsing.Utilities;

namespace Naninovel.Parsing;

internal class MixedValueParser (bool unwrap)
{
    private readonly List<IValueComponent> value = [];
    private readonly Queue<Token> expressions = new();
    private readonly Queue<Token> textIds = new();
    private readonly Queue<Token> textIdBodies = new();

    public void AddExpressionToken (Token token)
    {
        expressions.Enqueue(token);
    }

    public void AddTextIdToken (Token token)
    {
        textIds.Enqueue(token);
    }

    public void AddTextIdBodyToken (Token token)
    {
        textIdBodies.Enqueue(token);
    }

    public void ClearAddedExpressions ()
    {
        expressions.Clear();
    }

    public MixedValue Parse (Token valueToken, LineWalker walker, bool unescapeAuthor)
    {
        value.Clear();

        var unescapeQuotes = unwrap && IsValueWrapped();
        var startIndex = valueToken.Start + (unescapeQuotes ? 1 : 0);
        var endIndex = valueToken.EndIndex - (unescapeQuotes ? 1 : 0);

        var index = startIndex;
        var textStartIndex = -1;
        for (; index <= endIndex; index++)
            if (ShouldProcessExpression()) ProcessExpression();
            else if (ShouldProcessIdentifiedText()) ProcessIdentifiedText();
            else if (!IsTextStarted()) textStartIndex = index;
        if (IsTextStarted()) value.Add(ParseText(endIndex - textStartIndex + 1));

        textIds.Clear();
        textIdBodies.Clear();
        expressions.Clear();

        return new MixedValue(value);

        bool IsTextStarted () => textStartIndex != -1;

        bool IsValueWrapped ()
        {
            return walker.GetCharAt(valueToken.Start) == '\"' &&
                   walker.GetCharAt(valueToken.EndIndex) == '\"';
        }

        bool ShouldProcessExpression ()
        {
            return expressions.Count > 0 &&
                   expressions.Peek().Start == index;
        }

        bool ShouldProcessIdentifiedText ()
        {
            return textIds.Count > 0 &&
                   textIds.Peek().Start == index;
        }

        void ProcessExpression ()
        {
            var expression = expressions.Dequeue();
            if (IsTextStarted()) value.Add(ParseText(expression.Start - textStartIndex));
            value.Add(ParseExpression(expression));
            index = expression.EndIndex;
        }

        void ProcessIdentifiedText ()
        {
            var identifiedText = textIds.Dequeue();
            var bodyToken = textIdBodies.Count > 0 && textIdBodies.Peek().EndIndex <= identifiedText.EndIndex
                ? textIdBodies.Dequeue() : default(Token?);
            value.Add(ParseIdentifiedText(identifiedText, bodyToken));
            index = identifiedText.EndIndex;
        }

        Expression ParseExpression (Token expressionToken)
        {
            var bodyStart = expressionToken.Start + 1;
            var bodyLength = expressionToken.EndIndex - expressionToken.Start - 1;
            var body = new PlainText(walker.Extract(bodyStart, bodyLength));
            if (bodyLength > 0)
                walker.Associate(body, new InlineRange(bodyStart, bodyLength));
            var expression = new Expression(body);
            walker.Associate(expression, expressionToken);
            return expression;
        }

        PlainText ParseText (int length)
        {
            var text = walker.Extract(textStartIndex, length);
            if (unescapeAuthor) text = UnescapeAuthor(text);
            var plain = new PlainText(UnescapePlain(text, unescapeQuotes));
            walker.Associate(plain, new InlineRange(textStartIndex, length));
            textStartIndex = -1;
            return plain;
        }

        IdentifiedText ParseIdentifiedText (Token textIdToken, Token? textIdBodyToken)
        {
            var textStart = IsTextStarted() ? textStartIndex : textIdToken.Start;
            var textLength = textIdToken.Start - textStart;
            var text = textLength > 0 ? ParseText(textLength) : PlainText.Empty;
            var body = textIdBodyToken.HasValue ? new PlainText(walker.Extract(textIdBodyToken.Value)) : PlainText.Empty;
            if (textIdBodyToken.HasValue)
                walker.Associate(body, textIdBodyToken.Value);
            var textId = new TextIdentifier(body);
            walker.Associate(textId, textIdToken);
            var identifiedText = new IdentifiedText(text, textId);
            walker.Associate(identifiedText, new InlineRange(textStart, textLength + textIdToken.Length));
            return identifiedText;
        }
    }

    private static string UnescapePlain (string value, bool unescapeQuotes)
    {
        for (int i = value.Length - 2; i >= 0; i--)
            if (ShouldRemove(i))
                value = value.Remove(i, 1);
        return value;

        bool ShouldRemove (int i)
        {
            if (value[i] != '\\' || IsEscaped(value, i)) return false;
            var prev = value[i + 1];
            if (unescapeQuotes && prev == '"') return true;
            return IsPlainTextControlChar(prev, value.ElementAtOrDefault(i + 2));
        }
    }

    private static string UnescapeAuthor (string value)
    {
        const string target = $"\\{Identifiers.AuthorAssign}";
        var targetIndex = value.IndexOf(target, StringComparison.Ordinal);
        if (targetIndex < 1) return value;
        for (int i = 0; i < targetIndex; i++)
            if (char.IsWhiteSpace(value[i]))
                return value;
        return value.Remove(targetIndex, 1);
    }
}
