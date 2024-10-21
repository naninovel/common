using System.Globalization;

namespace Naninovel.Expression;

public static class OperandExtensions
{
    /// <summary>
    /// Returns underlying value of the operand, casted into specified type.
    /// </summary>
    /// <exception cref="Invalid">Thrown when cast to expected type is not possible.</exception>
    public static T GetValue<T> (this IOperand op) => (T)GetValue(op, typeof(T));

    /// <summary>
    /// Returns underlying value of the operand, casted into specified type.
    /// </summary>
    /// <exception cref="Invalid">Thrown when cast to expected type is not possible.</exception>
    public static object GetValue (this IOperand op, Type type)
    {
        var raw = op.GetValue();
        try { return Convert.ChangeType(raw, type, CultureInfo.InvariantCulture); }
        catch { throw new Error($"Unexpected operand type: {raw.GetType().Name} (expected {type.Name})"); }
    }
}
