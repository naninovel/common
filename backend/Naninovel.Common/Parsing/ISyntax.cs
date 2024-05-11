namespace Naninovel.Parsing;

/// <summary>
/// NaniScript syntax identifiers.
/// </summary>
public interface ISyntax
{
    /// <summary>
    /// Identifies start of <see cref="CommentLine"/>.
    /// </summary>
    string CommentLine { get; }
    /// <summary>
    /// Identifies start of <see cref="LabelLine"/>.
    /// </summary>
    string LabelLine { get; }
    /// <summary>
    /// Identifies start of <see cref="CommandLine"/>.
    /// </summary>
    string CommandLine { get; }
    /// <summary>
    /// Used to delimit author ID from the beginning of <see cref="GenericLine"/>.
    /// </summary>
    string AuthorAssign { get; }
    /// <summary>
    /// Used to delimit author appearance from author ID in <see cref="GenericLine"/>.
    /// </summary>
    string AuthorAppearance { get; }
    /// <summary>
    /// Identifies start of <see cref="Expression"/>.
    /// </summary>
    string ExpressionOpen { get; }
    /// <summary>
    /// Identifies end of <see cref="Expression"/>.
    /// </summary>
    string ExpressionClose { get; }
    /// <summary>
    /// Identifies start of <see cref="InlinedCommand"/>.
    /// </summary>
    string InlinedOpen { get; }
    /// <summary>
    /// Identifies end of <see cref="InlinedCommand"/>.
    /// </summary>
    string InlinedClose { get; }
    /// <summary>
    /// Used to delimit command parameter value from its name.
    /// </summary>
    string ParameterAssign { get; }
    /// <summary>
    /// Used to delimit items inside list parameter value.
    /// </summary>
    string ListDelimiter { get; }
    /// <summary>
    /// Used to delimit value from name inside named parameter value.
    /// </summary>
    string NamedDelimiter { get; }
    /// <summary>
    /// Identifies start of <see cref="TextIdentifier"/>.
    /// </summary>
    string TextIdOpen { get; }
    /// <summary>
    /// Identifies end of <see cref="TextIdentifier"/>.
    /// </summary>
    string TextIdClose { get; }
    /// <summary>
    /// When placed after boolean parameter name, identifies positive value and vice-versa.
    /// </summary>
    string BooleanFlag { get; }
    /// <summary>
    /// Positive boolean parameter value.
    /// </summary>
    string True { get; }
    /// <summary>
    /// Negative boolean parameter value.
    /// </summary>
    string False { get; }
}
