using System;

namespace Naninovel;

public class Debug
{
    private static ILogger? logger;

    public Debug (ILogger logger)
    {
        Debug.logger = logger;
    }

    public static void Log (string message)
    {
        logger?.Info(message);
    }

    public static void Trace (string? message = null)
    {
        Log($"{message ?? "Trace"}: {Environment.StackTrace}");
    }
}
