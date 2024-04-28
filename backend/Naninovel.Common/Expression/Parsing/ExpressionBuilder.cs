namespace Naninovel.Expression;

internal class ExpressionBuilder
{
    private readonly List<object> parts = [];

    public IExpression Build (IReadOnlyList<object> parts)
    {
        Reset(parts);

        return null!;
    }

    private void Reset (IReadOnlyList<object> parts)
    {
        this.parts.Clear();
        this.parts.AddRange(parts);
    }

    private void BuildUnary ()
    {

    }

    private void BuildTernary ()
    {

    }

    private void BuildAssociative ()
    {

    }

    private void BuildBinary ()
    {
        var binaryIdx = -1;
        for (var index = 0; index < parts.Count; index++)
        {
            var part = parts.ToArray()[index];
            if (part is IBinaryOperator op)
        }
    }
}
