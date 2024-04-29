namespace Naninovel.Expression;

/// <summary>
/// Lowest denominator of <see cref="IExpression"/>, to which
/// the expression is expected to eventually collapse. Can be either
/// <see cref="Numeric"/>, <see cref="String"/> or <see cref="Boolean"/>.
/// </summary>
public interface IOperand : IExpression
{
    /// <summary>
    /// Returns raw (un-typed) underlying value of the operand.
    /// </summary>
    object GetValue ();
}
