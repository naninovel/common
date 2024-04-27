namespace Naninovel.Expression;

/// <summary>
/// Artifact to be evaluated by <see cref="ExpressionEvaluator"/>.
/// </summary>
/// <remarks>
/// Can be either <see cref="BinaryOperation"/>, <see cref="UnaryOperation"/>,
/// <see cref="Function"/>, <see cref="Variable"/> or an <see cref="IOperand"/>.
/// </remarks>
public interface IExpression;
