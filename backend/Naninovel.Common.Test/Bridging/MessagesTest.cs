using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Naninovel.Bridging.Test;

public class MessagesTest
{
    [Fact] // TODO: Change to required when Unity allows and remove this test.
    [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
    public void MessagePropsInitializedWithDefaults ()
    {
        Assert.Null(new GotoRequest().PlaybackSpot);
        Assert.Null(new PlaybackSpot().ScriptName);
        Assert.Equal(0, new PlaybackSpot().LineIndex);
        Assert.Equal(0, new PlaybackSpot().InlineIndex);
        Assert.Null(new PlaybackStatus().PlayedSpot);
        Assert.False(new PlaybackStatus().Playing);
        Assert.Null(new UpdateMetadata().Metadata);
        Assert.Null(new UpdatePlaybackStatus().PlaybackStatus);
    }
}
