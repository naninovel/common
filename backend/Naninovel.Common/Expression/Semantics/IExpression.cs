namespace Naninovel.Expression;

/// <summary>
/// Artifact to be evaluated by <see cref="Evaluator"/>.
/// </summary>
/// <remarks>
/// Can be either <see cref="BinaryOperation"/>, <see cref="UnaryOperation"/>,
/// <see cref="TernaryOperation"/>, <see cref="Function"/>, <see cref="Variable"/>
/// or one of <see cref="IOperand"/> specs. All expressions are expected to
/// eventually collapse into either <see cref="IOperand"/> or <see cref="Invalid"/>.
/// </remarks>
public interface IExpression;
