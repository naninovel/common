using DotNetJS;

namespace Naninovel.Common.Bindings;

public partial class JSLogger : ILogger
{
    [JSFunction] public static partial void LogInfo (string message);
    [JSFunction] public static partial void LogWarning (string message);
    [JSFunction] public static partial void LogError (string message);

    public void Info (string message) => LogInfo(message);
    public void Warn (string message) => LogWarning(message);
    public void Error (string message) => LogError(message);
}
