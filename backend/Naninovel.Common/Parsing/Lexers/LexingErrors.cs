using System.Collections.Generic;

namespace Naninovel.Parsing;

public static class LexingErrors
{
    private static IReadOnlyDictionary<ErrorType, string> map { get; } = new Dictionary<ErrorType, string> {
        [ErrorType.SpaceInLabel] = "Label text cannot contain whitespace.",
        [ErrorType.MissingLabel] = "Label text cannot be empty.",
        [ErrorType.MissingCommandId] = "Command identifier is missing.",
        [ErrorType.MissingParamId] = "Parameter identifier is missing.",
        [ErrorType.MissingParamValue] = "Parameter value is missing.",
        [ErrorType.MultipleNameless] = "Multiple nameless parameters are not supported.",
        [ErrorType.MissingAppearance] = "Author appearance cannot be empty.",
        [ErrorType.MissingExpressionBody] = "Script expression body is missing.",
        [ErrorType.MissingTextIdBody] = "Text identifier body is missing."
    };

    public static string GetFor (ErrorType type) => map[type];
}
