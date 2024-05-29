namespace Naninovel.Metadata;

/// <summary>
/// Expression function.
/// </summary>
public class Function
{
    /// <summary>
    /// Identifier of the function.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Parameters of the function, in signature order.
    /// </summary>
    public FunctionParameter[] Parameters { get; set; } = [];
}
