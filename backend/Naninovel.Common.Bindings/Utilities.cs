using System;
using System.Text.Json.Serialization;
using DotNetJS;
using Naninovel.Common.Bindings;

[assembly: JSNamespace(Utilities.NamespacePattern, Utilities.NamespaceReplacement)]

namespace Naninovel.Common.Bindings;

public static partial class Utilities
{
    public const string NamespacePattern = @".*\.(\S+)";
    public const string NamespaceReplacement = "$1";

    public static void ConfigureJson ()
    {
        JS.Runtime.ConfigureJson(options => {
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.Converters.Add(new JsonStringEnumConverter());
        });
    }

    public static void Try (Action action)
    {
        try { action(); }
        catch (Exception e) { Log($"{action.GetType().Namespace} error: {e}"); }
    }

    public static T Try<T> (Func<T> func)
    {
        try { return func(); }
        catch (Exception e) { Log($"{func.GetType().Namespace} error: {e}"); }
        return default!;
    }

    [JSFunction] public static partial void Log (string message);
}
