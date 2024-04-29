namespace Naninovel.Expression;

public static class OperandExtensions
{
    /// <summary>
    /// Returns underlying value of the operand, casted into specified type.
    /// </summary>
    /// <exception cref="Error">Thrown when cast to expected type is not possible.</exception>
    public static T GetValue<T> (this IOperand op)
    {
        var raw = op.GetValue();
        if (raw is not T expected)
            throw new Error($"Unexpected operand type: {raw.GetType().Name} (expected {typeof(T).Name})");
        return expected;
    }
}
