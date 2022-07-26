namespace Naninovel.Parsing;

/// <summary>
/// Represents a part of a composite value.
/// </summary>
/// <remarks>
/// Composite value is either command parameter value or generic line text.
/// The value can contain multiple mixed parts,
/// such as <see cref="PlainText"/> and <see cref="Expression"/>.
/// </remarks>
public interface IMixedValue { }
