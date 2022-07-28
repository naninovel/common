using System.Collections.Generic;

namespace Naninovel.Parsing;

internal class CommandParser
{
    private readonly List<Parameter> parameters = new();
    private LineWalker walker;
    private string identifier = "";

    public Command Parse (LineWalker walker)
    {
        ResetState(walker);

        return new Command(identifier, parameters.ToArray());
    }

    private void ResetState (LineWalker walker)
    {
        this.walker = walker;
        parameters.Clear();
        identifier = "";
    }
}
