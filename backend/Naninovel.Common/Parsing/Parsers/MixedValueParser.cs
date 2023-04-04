using System.Collections.Generic;
using static Naninovel.Parsing.Utilities;

namespace Naninovel.Parsing;

internal class MixedValueParser
{
    private readonly bool unwrap;
    private readonly List<IValueComponent> value = new();
    private readonly Queue<Token> expressions = new();

    public MixedValueParser (bool unwrap)
    {
        this.unwrap = unwrap;
    }

    public void AddExpressionToken (Token expressionToken)
    {
        expressions.Enqueue(expressionToken);
    }

    public void ClearAddedExpressions ()
    {
        expressions.Clear();
    }

    public MixedValue Parse (Token valueToken, LineWalker walker)
    {
        value.Clear();

        var unescapeQuotes = unwrap && IsValueWrapped();
        var startIndex = valueToken.StartIndex + (unescapeQuotes ? 1 : 0);
        var endIndex = valueToken.EndIndex - (unescapeQuotes ? 1 : 0);

        var index = startIndex;
        var textStartIndex = -1;
        for (; index <= endIndex; index++)
            if (ShouldProcessExpression()) ProcessExpression();
            else if (!IsTextStarted()) textStartIndex = index;
        if (IsTextStarted()) AddText(endIndex - textStartIndex + 1);
        return new MixedValue(value);

        bool IsValueWrapped ()
        {
            return walker.GetCharAt(valueToken.StartIndex) == '\"' &&
                   walker.GetCharAt(valueToken.EndIndex) == '\"';
        }

        bool ShouldProcessExpression ()
        {
            return expressions.Count > 0 &&
                   expressions.Peek().StartIndex == index;
        }

        void ProcessExpression ()
        {
            var expression = expressions.Dequeue();
            if (IsTextStarted()) AddText(expression.StartIndex - textStartIndex);
            AddExpression(expression);
            index = expression.EndIndex;
        }

        bool IsTextStarted () => textStartIndex != -1;

        void AddText (int length)
        {
            var text = walker.Extract(textStartIndex, length);
            var plain = new PlainText(UnescapePlain(text, unescapeQuotes));
            walker.Associate(plain, new LineRange(textStartIndex, length));
            value.Add(plain);
            textStartIndex = -1;
        }

        void AddExpression (Token expressionToken)
        {
            var bodyStart = expressionToken.StartIndex + 1;
            var bodyLength = expressionToken.EndIndex - expressionToken.StartIndex - 1;
            var body = new PlainText(walker.Extract(bodyStart, bodyLength));
            if (bodyLength > 0)
                walker.Associate(body, new LineRange(bodyStart, bodyLength));
            var expression = new Expression(body);
            walker.Associate(expression, expressionToken);
            value.Add(expression);
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
            var prevChar = value[i + 1];
            var nextChar = i == 0 ? default : value[i - 1];
            return unescapeQuotes && prevChar == '"' || IsControlChar(prevChar, nextChar);
        }
    }
}
