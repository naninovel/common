using Xunit;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing.Test;

public class TokenResolverTest
{
    private readonly ParseTestHelper<CommentLine> commentParser = new((e, a) => new CommentLineParser(e, a).Parse);
    private readonly ParseTestHelper<LabelLine> labelParser = new((e, a) => new LabelLineParser(e, a).Parse);
    private readonly ParseTestHelper<CommandLine> commandParser = new((e, a) => new CommandLineParser(e, a).Parse);
    private readonly ParseTestHelper<GenericLine> genericParser = new((e, a) => new GenericLineParser(e, a).Parse);

    [Fact]
    public void CommentLineComponentsAssociatedCorrectly ()
    {
        var line = commentParser.Parse("; comment");
        Assert.Equal(new(CommentText, 2, 7), commentParser.Resolve(line.Comment));
    }

    [Fact]
    public void LabelLineComponentsAssociatedCorrectly ()
    {
        var line = labelParser.Parse("# label");
        Assert.Equal(new(LabelText, 2, 5), labelParser.Resolve(line.Label));
    }

    [Fact]
    public void CommandLineComponentsAssociatedCorrectly ()
    {
        var line = commandParser.Parse("@c v p:v{x}");
        Assert.Equal(new(CommandBody, 1, 10), commandParser.Resolve(line.Command));
        Assert.Equal(new(CommandId, 1, 1), commandParser.Resolve(line.Command.Identifier));
        Assert.Equal(new(NamelessParam, 3, 1), commandParser.Resolve(line.Command.Parameters[0]));
        Assert.Equal(new(ParamValue, 3, 1), commandParser.Resolve(line.Command.Parameters[0].Value[0] as PlainText));
        Assert.Equal(new(NamedParam, 5, 6), commandParser.Resolve(line.Command.Parameters[1]));
        Assert.Equal(new(ParamId, 5, 1), commandParser.Resolve(line.Command.Parameters[1].Identifier));
        Assert.Equal(new(ParamValue, 7, 1), commandParser.Resolve(line.Command.Parameters[1].Value[0] as PlainText));
        Assert.Equal(new(TokenType.Expression, 8, 3), commandParser.Resolve(line.Command.Parameters[1].Value[0] as Expression));
        Assert.Equal(new(ExpressionBody, 9, 1), commandParser.Resolve((line.Command.Parameters[1].Value[0] as Expression)!.Body));
    }

    [Fact]
    public void GenericLineComponentsAssociatedCorrectly ()
    {
        var line = genericParser.Parse("k.h: x{e}[i {e}]");
        Assert.Equal(new(AuthorId, 0, 1), genericParser.Resolve(line.Prefix.Author));
        Assert.Equal(new(AuthorAppearance, 2, 1), genericParser.Resolve(line.Prefix.Appearance));
        Assert.Equal(new(TokenType.GenericText, 5, 1), genericParser.Resolve((line.Content[0] as GenericText)!.Text[0] as PlainText));
        Assert.Equal(new(TokenType.Expression, 6, 3), genericParser.Resolve((line.Content[0] as GenericText)!.Text[1] as Expression));
        Assert.Equal(new(ExpressionBody, 7, 1), genericParser.Resolve(((line.Content[0] as GenericText)!.Text[1] as Expression)!.Body));
        Assert.Equal(new(Inlined, 9, 7), genericParser.Resolve(line.Content[1] as InlinedCommand));
        Assert.Equal(new(CommandBody, 10, 5), genericParser.Resolve((line.Content[1] as InlinedCommand)!.Command));
        Assert.Equal(new(CommandId, 10, 1), genericParser.Resolve((line.Content[1] as InlinedCommand)!.Command.Identifier));
        Assert.Equal(new(CommandId, 12, 3), genericParser.Resolve((line.Content[1] as InlinedCommand)!.Command.Parameters[0]));
        Assert.Equal(new(TokenType.Expression, 12, 3), genericParser.Resolve((line.Content[1] as InlinedCommand)!.Command.Parameters[0].Value[0] as Expression));
        Assert.Equal(new(ExpressionBody, 13, 1), genericParser.Resolve(((line.Content[1] as InlinedCommand)!.Command.Parameters[0].Value[0] as Expression)!.Body));
    }
}
