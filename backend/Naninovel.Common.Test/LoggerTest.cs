﻿using System;
using Moq;
using Xunit;

namespace Naninovel.Test;

public class LoggerTest
{
    [Fact]
    public void LogExceptionInvokesLogErrorWithException ()
    {
        var logger = new Mock<ILogger>();
        logger.Object.Exception(new Exception("foo"));
        logger.Verify(l => l.Error(It.Is<string>(s => s.Contains("foo"))));
    }
}