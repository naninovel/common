using System;
using System.Text.Json.Serialization;
using DotNetJS;
using Naninovel.Common.Bindings;

[assembly: JSNamespace(Utilities.NamespacePattern, Utilities.NamespaceReplacement)]

namespace Naninovel.Common.Bindings;

public static class Utilities
{
    public const string NamespacePattern = @".*\.I?(?:JS)?(\S+)";
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
        catch (Exception e) { LogException(action, e); }
    }

    public static void Try<T1> (Action<T1> action, T1 arg1)
    {
        try { action(arg1); }
        catch (Exception e) { LogException(action, e); }
    }

    public static void Try<T1, T2> (Action<T1, T2> action, T1 arg1, T2 arg2)
    {
        try { action(arg1, arg2); }
        catch (Exception e) { LogException(action, e); }
    }

    public static void Try<T1, T2, T3> (Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
    {
        try { action(arg1, arg2, arg3); }
        catch (Exception e) { LogException(action, e); }
    }

    public static void Try<T1, T2, T3, T4> (Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        try { action(arg1, arg2, arg3, arg4); }
        catch (Exception e) { LogException(action, e); }
    }

    public static void Try<T1, T2, T3, T4, T5> (Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        try { action(arg1, arg2, arg3, arg4, arg5); }
        catch (Exception e) { LogException(action, e); }
    }

    public static TResult Try<TResult> (Func<TResult> func)
    {
        try { return func(); }
        catch (Exception e) { LogException(func, e); }
        return default!;
    }

    public static TResult Try<T1, TResult> (Func<T1, TResult> func, T1 arg1)
    {
        try { return func(arg1); }
        catch (Exception e) { LogException(func, e); }
        return default!;
    }

    public static TResult Try<T1, T2, TResult> (Func<T1, T2, TResult> func, T1 arg1, T2 arg2)
    {
        try { return func(arg1, arg2); }
        catch (Exception e) { LogException(func, e); }
        return default!;
    }

    public static TResult Try<T1, T2, T3, TResult> (Func<T1, T2, T3, TResult> func, T1 arg1, T2 arg2, T3 arg3)
    {
        try { return func(arg1, arg2, arg3); }
        catch (Exception e) { LogException(func, e); }
        return default!;
    }

    public static TResult Try<T1, T2, T3, T4, TResult> (Func<T1, T2, T3, T4, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        try { return func(arg1, arg2, arg3, arg4); }
        catch (Exception e) { LogException(func, e); }
        return default!;
    }

    public static TResult Try<T1, T2, T3, T4, T5, TResult> (Func<T1, T2, T3, T4, T5, TResult> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        try { return func(arg1, arg2, arg3, arg4, arg5); }
        catch (Exception e) { LogException(func, e); }
        return default!;
    }

    private static void LogException (object func, Exception e)
    {
        JSLogger.LogError($"{func.GetType().Namespace} error: {e}");
    }
}
