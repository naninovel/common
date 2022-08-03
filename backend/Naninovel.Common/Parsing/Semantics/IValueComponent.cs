namespace Naninovel.Parsing;

/// <summary>
/// Represents a part of a <see cref="MixedValue"/>.
/// </summary>
/// <example>
/// Consider the following mixed value: <c>text1{expr}text2</c><br/>
/// In the mixed collection it will represented as:<br/>
/// [0] <see cref="PlainText"/>("text1")<br/>
/// [1] <see cref="Expression"/>("expr")<br/>
/// [2] <see cref="PlainText"/>("text2")
/// </example>
public interface IValueComponent { }
