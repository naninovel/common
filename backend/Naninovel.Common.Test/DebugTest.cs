using Moq;

namespace Naninovel.Test;

public class DebugTest
{
    private readonly Mock<ILogger> logger = new();

    [Fact]
    public void WhenNotConstructedDoesntLog ()
    {
        Debug.Log("foo");
        Debug.Trace();
        logger.VerifyNoOtherCalls();
    }

    [Fact]
    public void LogsInfoWhenConstructed ()
    {
        _ = new Debug(logger.Object);
        Debug.Log("foo");
        Debug.Trace("bar");
        logger.Verify(l => l.Info("foo"));
        logger.Verify(l => l.Info(It.Is<string>(s => s.Contains("bar"))));
    }

    [Fact]
    public void WhenTracingWithoutMessagePrependsTrace ()
    {
        _ = new Debug(logger.Object);
        Debug.Trace();
        logger.Verify(l => l.Info(It.Is<string>(s => s.Contains("Trace"))));
    }
}
