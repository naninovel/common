namespace Naninovel.Expression;

/// <summary>
/// Exception thrown from Naninovel expression processing internals.
/// </summary>
public class Error (string message, int? index = null, int? length = null)
    : Naninovel.Error(message)
{
    public int? Index { get; } = index;
    public int? Length { get; } = length;
}
