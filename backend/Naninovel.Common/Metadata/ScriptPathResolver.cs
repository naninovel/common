namespace Naninovel.Metadata;

/// <summary>
/// Allows resolving local resource path of scenario scripts from their file's URI.
/// </summary>
/// <remarks>
/// Local script path is, by convention, script file name w/o the extension with parent
/// directories prepended with forward slashes up to <see cref="RootUri"/>.
/// </remarks>
public class ScriptPathResolver
{
    /// <summary>
    /// Location of the directory where all the scripts are stored.
    /// </summary>
    public string RootUri { get => rootUri; set => rootUri = FormatRootUri(value); }

    private string rootUri = "/";

    /// <summary>
    /// Resolves local resource path of a scenario script with specified file location.
    /// </summary>
    /// <remarks>
    /// When specified path doesn't start with <see cref="RootUri"/>, will keep
    /// all the parent directories after the first forward slash; in case path doesn't
    /// contain single forward slash, will return it as-is, but w/o the .nani extension.
    /// </remarks>
    public string Resolve (string fileUri)
    {
        fileUri = FormatUri(fileUri);
        var localUri = fileUri.GetAfterFirst(rootUri);
        if (string.IsNullOrEmpty(localUri))
            localUri = fileUri.GetAfterFirst("/");
        if (string.IsNullOrEmpty(localUri))
            localUri = fileUri;
        if (!localUri.EndsWithOrdinal(".nani")) return localUri;
        return localUri.GetBeforeLast(".nani");
    }

    private static string FormatUri (string content)
    {
        return content.Replace("\\", "/");
    }

    private static string FormatRootUri (string content)
    {
        content = FormatUri(content);
        if (!content.EndsWithOrdinal("/"))
            content += "/";
        return content;
    }
}
