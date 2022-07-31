namespace Naninovel.Parsing;

/// <summary>
/// Represents a part of a composite value.
/// </summary>
/// <remarks>
/// The value can be either <see cref="PlainText"/> or <see cref="Expression"/>.
/// </remarks>
/// <example>
/// Consider the following composite value: <c>text1{expr}text2</c><br/>
/// In <see cref="IMixedValue"/> collection it will be represented as:<br/>
/// [0] <see cref="PlainText"/>("text1")<br/>
/// [1] <see cref="Expression"/>("expr")<br/>
/// [2] <see cref="PlainText"/>("text2")
/// </example>
public interface IMixedValue { }
