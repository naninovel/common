namespace Naninovel;

/// <summary>
/// Common path and URI related helpers and extensions.
/// </summary>
public static class PathUtil
{
    /// <summary>
    /// Resolves script name (unique identifier) from specified script file path or URI.
    /// </summary>
    /// <remarks>
    /// By convention, script unique identifier is the file name without the extension;
    /// this implies there can't be script files with equal names under single project.
    /// </remarks>
    public static string ResolveScriptName (string scriptUri)
    {
        return Path.GetFileNameWithoutExtension(scriptUri);
    }
}
