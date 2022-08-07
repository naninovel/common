using System;

namespace Naninovel;

/// <summary>
/// Represents error occured in Naninovel internal behaviour.
/// </summary>
public class Error : Exception
{
    public Error () { }
    public Error (string message) : base(message) { }
    public Error (string message, Exception inner) : base(message, inner) { }
}
