using System.Collections.Generic;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

public class GenericLineParser
{
    private readonly CommandParser commandParser = new();
    private readonly MixedValueParser valueParser = new(false);
    private readonly List<IGenericContent> content = new();
    private readonly LineWalker walker;
    private GenericPrefix prefix;

    public GenericLineParser (IErrorHandler errorHandler = null, IAssociator associator = null)
    {
        walker = new(errorHandler, associator);
    }

    public GenericLine Parse (string lineText, IReadOnlyList<Token> tokens)
    {
        ResetState(lineText, tokens);
        while (TryNext()) continue;
        return new GenericLine(prefix, content.ToArray());
    }

    private void ResetState (string lineText, IReadOnlyList<Token> tokens)
    {
        walker.Reset(lineText, tokens);
        content.Clear();
        prefix = null;
    }

    private bool TryNext ()
    {
        if (!walker.Next(out var token)) return false;

        switch (token.Type)
        {
            case AuthorId:
                prefix = new GenericPrefix(new(walker.Extract(token)));
                return true;
            case AuthorAppearance:
                if (prefix?.Author != null)
                    prefix = new GenericPrefix(prefix.Author, new(walker.Extract(token)));
                return true;
            case AuthorAssign:
                valueParser.ClearAddedExpressions();
                return true;
            case TokenType.GenericText:
                AddText(token);
                return true;
            case TokenType.Expression:
                valueParser.AddExpressionToken(token);
                return true;
            case InlinedOpen:
                AddInlined();
                return true;
            case Error:
                walker.Error(token);
                return true;
            default: return true;
        }

        void AddText (Token textToken)
        {
            var value = valueParser.Parse(textToken, walker);
            var text = new GenericText(value);
            content.Add(text);
            valueParser.ClearAddedExpressions();
        }

        void AddInlined ()
        {
            var command = commandParser.Parse(walker);
            var inlined = new InlinedCommand(command);
            content.Add(inlined);
        }
    }
}
