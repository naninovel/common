using System.Diagnostics.CodeAnalysis;

namespace Naninovel.Bridging.Test;

public class MessagesTest
{
    [Fact] // TODO: Change to required when Unity allows and remove this test.
    [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
    public void MessagePropsInitializedWithDefaults ()
    {
        Assert.Null(new GotoRequest().PlaybackSpot);
        Assert.Null(new PlaybackSpot().ScriptPath);
        Assert.Equal(0, new PlaybackSpot().LineIndex);
        Assert.Equal(0, new PlaybackSpot().InlineIndex);
        Assert.Null(new PlaybackStatus().PlayedSpot);
        Assert.False(new PlaybackStatus().Playing);
        Assert.Null(new UpdateMetadata().Metadata);
        Assert.Null(new UpdatePlaybackStatus().PlaybackStatus);
    }
}
