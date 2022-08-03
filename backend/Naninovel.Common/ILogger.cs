using System;

namespace Naninovel;

public interface ILogger
{
    void Info (string message);
    void Warn (string message);
    void Error (string message);
}

public static class LoggerExtensions
{
    public static void Exception (this ILogger logger, Exception exception)
    {
        logger.Error(exception.ToString());
    }
}
