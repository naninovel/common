using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Naninovel.Bridging.Test;

public class MessagesTest
{
    [Fact] // TODO: Change to required when Unity allows and remove this test.
    [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
    public void MessagePropsAreNullByDefault ()
    {
        Assert.Null(new GotoRequest().PlaybackSpot);
        Assert.Null(new PlaybackSpot().ScriptName);
        Assert.Null(new PlaybackStatus().PlayedSpot);
        Assert.Null(new UpdateMetadata().Metadata);
        Assert.Null(new UpdatePlaybackStatus().PlaybackStatus);
    }
}
