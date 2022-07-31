using System.Collections.Generic;

namespace Naninovel.Parsing;

internal class MixedValueParser
{
    private readonly bool unwrap;
    private readonly List<IMixedValue> value = new();
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

    public IMixedValue[] Parse (Token valueToken, LineWalker walker)
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
        return value.ToArray();

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

        void AddText (int endIndex)
        {
            var text = walker.Extract(textStartIndex, endIndex);
            var decoded = ValueCoder.UnescapeMixed(text, unescapeQuotes);
            value.Add(new PlainText(decoded));
            textStartIndex = -1;
        }

        void AddExpression (Token expression)
        {
            var startIndex = expression.StartIndex + 1;
            var length = expression.EndIndex - expression.StartIndex - 1;
            value.Add(new Expression(new(walker.Extract(startIndex, length))));
        }
    }
}
