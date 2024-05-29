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
    /// Optional documentation, when specified or null.
    /// </summary>
    public string? Summary { get; set; }
    /// <summary>
    /// Optional remarks, when specified or null.
    /// </summary>
    public string? Remarks { get; set; }
    /// <summary>
    /// Optional usage examples, when specified or null.
    /// </summary>
    public string? Example { get; set; }
    /// <summary>
    /// Parameters of the function, in signature order.
    /// </summary>
    public FunctionParameter[] Parameters { get; set; } = [];
}
