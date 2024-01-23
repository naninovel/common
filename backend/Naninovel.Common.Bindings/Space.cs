namespace Naninovel.Bindings;

public static class Space
{
    public const string Pattern = @".*?([^\.]+?)(?:UI)?$";
    public const string Replacement = "$1";
}
