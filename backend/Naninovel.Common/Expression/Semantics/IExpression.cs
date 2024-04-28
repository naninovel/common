namespace Naninovel.Expression;

/// <summary>
/// Artifact to be evaluated by <see cref="Evaluator"/>.
/// </summary>
/// <remarks>
/// Can be either <see cref="BinaryOperation"/>, <see cref="UnaryOperation"/>,
/// <see cref="TernaryOperation"/>, <see cref="Function"/>, <see cref="Variable"/>,
/// <see cref="Map"/> or one of <see cref="IOperand"/>. All expressions are expected
/// to eventually collapse into <see cref="IOperand"/>.
/// </remarks>
public interface IExpression;
