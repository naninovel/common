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
    /// Relative (to the project root) location of a directory where all the
    /// script files are stored; starts with '/' but doesn't end with '/'.
    /// </summary>
    public string RootUri { get => rootUri; set => SetRootUri(value); }

    private readonly Dictionary<(string RootUri, string FileUri), string> memo = [];
    private string rootUri = "/";
    private string rootPrefix = "/";

    /// <summary>
    /// Resolves local resource path of a scenario script with specified file location.
    /// Results are memoized on consequent requests with same file and root URIs.
    /// </summary>
    /// <remarks>
    /// When specified path doesn't contain <see cref="RootUri"/>, will keep
    /// all the parent directories after the first separator; in case path doesn't
    /// contain separators, returns it as-is, but w/o the .nani extension.
    /// </remarks>
    public string Resolve (string fileUri)
    {
        var key = (RootUri, fileUri);
        if (memo.TryGetValue(key, out var memoized)) return memoized;

        fileUri = FormatUri(fileUri);
        var localUri = fileUri.GetAfterFirst(rootPrefix);
        if (string.IsNullOrEmpty(localUri))
            localUri = fileUri.GetAfterFirst("/");
        if (string.IsNullOrEmpty(localUri))
            localUri = fileUri;

        var path = localUri.EndsWithOrdinal(".nani") ? localUri[..^5] : localUri;
        return memo[key] = path;
    }

    private static string FormatUri (string content)
    {
        return content.Replace("\\", "/");
    }

    private void SetRootUri (string uri)
    {
        uri = FormatUri(uri).Trim();
        if (!uri.StartsWith('/')) uri = $"/{uri}";
        if (uri.Length > 1 && uri.EndsWith('/')) uri = uri[..^1];
        rootUri = uri;
        rootPrefix = $"{rootUri[1..]}/";
    }
}
