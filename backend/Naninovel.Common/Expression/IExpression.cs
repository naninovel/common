namespace Naninovel.Expression;

/// <summary>
/// Artifact to be evaluated by <see cref="ExpressionEvaluator"/>.
/// </summary>
/// <remarks>
/// Can be either <see cref="BinaryOperation"/>, <see cref="Function"/>,
/// <see cref="Variable"/> or one of the <see cref="IOperand"/> specs.
/// </remarks>
public interface IExpression;
