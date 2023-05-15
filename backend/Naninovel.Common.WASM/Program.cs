using System;
using DotNetJS;
using static Naninovel.Bindings.Utilities;

[assembly: JSNamespace(NamespacePattern, NamespaceReplacement)]

namespace Naninovel.WASM;

public static class Program
{
    static Program () => ConfigureJson();

    public static void Main ()
    {
        // dotnet publish fails due to stripped System.Array otherwise
        _ = typeof(Array).AssemblyQualifiedName;
    }
}
