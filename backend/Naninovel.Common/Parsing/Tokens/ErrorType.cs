using System;

namespace Naninovel.Parsing;

[Flags]
public enum ErrorType
{
    SpaceInLabel = 1 << 0,
    MissingLabel = 1 << 1,
    MissingCommandId = 1 << 2,
    MissingParamId = 1 << 3,
    MissingParamValue = 1 << 4,
    MultipleNameless = 1 << 5,
    MissingAppearance = 1 << 6,
    MissingExpressionBody = 1 << 7,
    MissingTextIdBody = 1 << 8,
    ExpressionInGenericPrefix = 1 << 9
}

public static class ErrorTypeExtensions
{
    public static bool HasFlag (this ErrorType type, ErrorType flag)
    {
        return (type & flag) != 0;
    }
}
