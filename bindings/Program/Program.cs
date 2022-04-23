using DotNetJS;
using Naninovel.Common.Bindings.Bridging;
using Naninovel.Common.Bindings.Language;
using static Naninovel.Common.Bindings.Utilities;

[assembly: JSNamespace(NamespacePattern, NamespaceReplacement)]

namespace Bindings;

public static class Program
{
    static Program () => ConfigureJson();

    public static void Main ()
    {
        _ = typeof(Bridging);
        _ = typeof(Language);
    }
}
