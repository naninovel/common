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

    private readonly Dictionary<(string RootUri, string FileUri), string> memo = [];
    private string rootUri = "/";

    /// <summary>
    /// Resolves local resource path of a scenario script with specified file location.
    /// Results are memoized on consequent requests with same file and root URIs.
    /// </summary>
    /// <remarks>
    /// When specified path doesn't start with <see cref="RootUri"/>, will keep
    /// all the parent directories after the first forward slash; in case path doesn't
    /// contain single forward slash, will return it as-is, but w/o the .nani extension.
    /// </remarks>
    public string Resolve (string fileUri)
    {
        var key = (rootUri, fileUri);
        if (memo.TryGetValue(key, out var path)) return path;

        fileUri = FormatUri(fileUri);
        var localUri = fileUri.GetAfterFirst(rootUri);
        if (string.IsNullOrEmpty(localUri))
            localUri = fileUri.GetAfterFirst("/");
        if (string.IsNullOrEmpty(localUri))
            localUri = fileUri;
        path = localUri.EndsWithOrdinal(".nani")
            ? localUri.GetBeforeLast(".nani") : localUri;

        memo[key] = path;
        return path;
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
