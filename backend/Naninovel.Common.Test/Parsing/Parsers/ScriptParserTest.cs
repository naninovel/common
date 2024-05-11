namespace Naninovel.Parsing.Test;

public class ScriptParserTest
{
    private readonly ErrorCollector errors = [];
    private readonly ScriptParser parser;

    public ScriptParserTest ()
    {
        var options = new ParseOptions {
            Syntax = Syntax.Default,
            Handlers = new ParseHandlers { ErrorHandler = errors }
        };
        parser = new(options);
    }

    [Fact]
    public void WhenSplitNullReturnsEmptyString ()
    {
        var lines = ScriptParser.SplitText(null);
        Assert.Single(lines);
        Assert.Equal("", lines[0]);
    }

    [Fact]
    public void TextSplitsCorrectly ()
    {
        var lines = ScriptParser.SplitText(" 0 \n\r \r\n3 ");
        Assert.Equal(4, lines.Length);
        Assert.Equal(" 0 ", lines[0]);
        Assert.Equal("", lines[1]);
        Assert.Equal(" ", lines[2]);
        Assert.Equal("3 ", lines[3]);
    }

    [Fact]
    public void ScriptTextParsed ()
    {
        var lines = parser.ParseText(
            """

            ; Comment line
            @commandLine
            Generic text line
            # LabelLine
            """);
        Assert.IsType<GenericLine>(lines[0]);
        Assert.IsType<CommentLine>(lines[1]);
        Assert.IsType<CommandLine>(lines[2]);
        Assert.IsType<GenericLine>(lines[3]);
        Assert.IsType<LabelLine>(lines[4]);
    }

    [Fact]
    public void CanOverrideDefaultIdentifiers ()
    {
        var parser = new ScriptParser(new ParseOptions {
            Syntax = new Syntax {
                CommentLine = "/",
                CommandLine = ":",
                LabelLine = "%",
                ParameterAssign = "=",
                InlinedOpen = "(",
                InlinedClose = ")",
                True = "да",
                False = "нет"
            }
        });
        var lines = parser.ParseText(
            """

            / Comment line
            :commandLine p=v p! !p
            Generic \(text\) line (cmd)
            % LabelLine
            """);
        Assert.IsType<GenericLine>(lines[0]);
        Assert.Equal("Comment line", ((CommentLine)lines[1]).Comment);
        Assert.Equal("commandLine", ((CommandLine)lines[2]).Command.Identifier);
        Assert.Equal("p", ((CommandLine)lines[2]).Command.Parameters[0].Identifier);
        Assert.Equal("v", (PlainText)((CommandLine)lines[2]).Command.Parameters[0].Value[0]);
        Assert.Equal("да", (PlainText)((CommandLine)lines[2]).Command.Parameters[1].Value[0]);
        Assert.Equal("нет", (PlainText)((CommandLine)lines[2]).Command.Parameters[2].Value[0]);
        Assert.Equal("Generic (text) line ", (PlainText)((MixedValue)((GenericLine)lines[3]).Content[0])[0]);
        Assert.Equal("cmd", ((InlinedCommand)((GenericLine)lines[3]).Content[1]).Command.Identifier);
        Assert.Equal("LabelLine", ((LabelLine)lines[4]).Label);
    }

    [Fact]
    public void LexingErrorPreserved ()
    {
        parser.ParseLine("# x  x");
        Assert.Single(errors);
        Assert.Equal(LexingErrors.GetFor(ErrorType.SpaceInLabel), errors[0].Message);
        Assert.Equal(3, errors[0].StartIndex);
        Assert.Equal(2, errors[0].Length);
        Assert.Equal(4, errors[0].EndIndex);
    }

    [Fact]
    public void CanCreateParserWithoutHandlers ()
    {
        _ = new ScriptParser();
    }

    [Fact]
    public void IndentsParsedCorrectly ()
    {
        Assert.Equal(0, parser.ParseLine("# x").Indent);
        Assert.Equal(1, parser.ParseLine("    ; x").Indent);
        Assert.Equal(2, parser.ParseLine("        @c").Indent);
        Assert.Equal(2, parser.ParseLine("    \t    x").Indent);
    }
}
