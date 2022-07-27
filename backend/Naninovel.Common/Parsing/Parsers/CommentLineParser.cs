using System.Collections.Generic;

namespace Naninovel.Parsing;

public class CommentLineParser
{
    public CommentLine Parse (string lineText, IReadOnlyList<Token> tokens, ICollection<ParseError> errors = null)
    {
        var line = new CommentLine("");
        return line;
    }
}
