using static Naninovel.Parsing.TokenType;
using static Naninovel.Parsing.ParsingErrors;

namespace Naninovel.Parsing;

public class CommandLineParser : LineParser<CommandLine>
{
    private readonly CommandParser commandParser = new();

    protected override void Parse (CommandLine line)
    {
        if (!TryNext(LineId, out _)) AddError(MissingLineId);
        else commandParser.ParseNext(State, line.Command);
    }

    protected override void ClearLine (CommandLine line)
    {
        commandParser.ClearCommand(line.Command);
    }
}
