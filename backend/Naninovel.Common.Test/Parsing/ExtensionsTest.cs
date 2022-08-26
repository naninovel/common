using System;
using Xunit;

namespace Naninovel.Parsing.Test;

public class ExtensionsTest
{
    [Fact]
    public void CanResolveLineType ()
    {
        Assert.Equal(LineType.Comment, new CommentLine("").GetLineType());
        Assert.Equal(LineType.Label, new LabelLine("").GetLineType());
        Assert.Equal(LineType.Command, new CommandLine(new("")).GetLineType());
        Assert.Equal(LineType.Generic, new GenericLine(Array.Empty<IGenericContent>()).GetLineType());
    }

    [Fact]
    public void WhenResolvingUnknownLineTypeThrows ()
    {
        Assert.Throws<Error>(() => Extensions.GetLineType(null));
    }
}
