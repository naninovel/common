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
    /// Documentation for the function, when specified or null.
    /// </summary>
    public Documentation? Documentation { get; set; }
    /// <summary>
    /// Parameters of the function, in signature order.
    /// </summary>
    public FunctionParameter[] Parameters { get; set; } = [];
}
