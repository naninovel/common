using System;

namespace Naninovel;

public interface ILogger
{
    void Info (string message);
    void Warn (string message);
    void Error (string message);
    void Exception (Exception exception) => Error(exception.ToString());
}
