using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

public class GenericLineParser (ParseHandlers handlers)
{
    private readonly CommandParser commandParser = new();
    private readonly MixedValueParser valueParser = new(false);
    private readonly List<IGenericContent> content = [];
    private readonly LineWalker walker = new(handlers);
    private PlainText? authorId, authorAppearance;
    private GenericPrefix? prefix;

    public GenericLine Parse (string lineText, IReadOnlyList<Token> tokens)
    {
        Reset(lineText, tokens);
        while (TryNext()) continue;
        return new GenericLine(prefix, content.ToArray());
    }

    private void Reset (string lineText, IReadOnlyList<Token> tokens)
    {
        walker.Reset(lineText, tokens);
        content.Clear();
        authorId = authorAppearance = null;
        prefix = null;
    }

    private bool TryNext ()
    {
        if (!walker.Next(out var token)) return false;

        switch (token.Type)
        {
            case AuthorId:
                ParseAuthorId(token);
                return true;
            case AuthorAppearance:
                ParseAuthorAppearance(token);
                return true;
            case AuthorAssign:
                ParsePrefix(token);
                return true;
            case GenericText:
                ParseGenericText(token);
                return true;
            case TokenType.Expression:
                valueParser.AddExpressionToken(token);
                return true;
            case TextIdBody:
                valueParser.AddTextIdBodyToken(token);
                return true;
            case TextId:
                valueParser.AddTextIdToken(token);
                return true;
            case InlinedOpen:
                ParseInlined();
                return true;
            case Inlined:
                AssociateInlined(token);
                return true;
            case TokenType.Error:
                walker.Error(token);
                return true;
            default: return true;
        }
    }

    private void ParseAuthorId (Token authorIdToken)
    {
        authorId = new PlainText(walker.Extract(authorIdToken));
        walker.Associate(authorId, authorIdToken);
    }

    private void ParseAuthorAppearance (Token authorAppearanceToken)
    {
        authorAppearance = new PlainText(walker.Extract(authorAppearanceToken));
        walker.Associate(authorAppearance, authorAppearanceToken);
    }

    private void ParsePrefix (Token authorAssignToken)
    {
        valueParser.ClearAddedExpressions();
        prefix = new GenericPrefix(authorId!, authorAppearance);
        walker.Associate(prefix, new InlineRange(0, authorAssignToken.EndIndex + 1));
    }

    private void ParseGenericText (Token textToken)
    {
        var text = valueParser.Parse(textToken, walker, content.Count == 0);
        content.Add(text);
        walker.Associate(text, textToken);
        walker.Identify(text);
    }

    private void ParseInlined ()
    {
        var command = commandParser.Parse(walker);
        var inlined = new InlinedCommand(command);
        content.Add(inlined);
    }

    private void AssociateInlined (Token inlinedToken)
    {
        if (content.LastOrDefault() is InlinedCommand inlined)
            walker.Associate(inlined, inlinedToken);
    }
}
