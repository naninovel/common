using System.Collections.Generic;
using Moq;
using Xunit;

namespace Naninovel.Observing.Test;

public class RegistryTest
{
    [Fact]
    public void AddsOnRegister ()
    {
        var observer = new Mock<IMockObserver>();
        var collection = new List<IMockObserver>();
        var registry = new ObserverRegistry<IMockObserver>(collection);
        registry.Register(observer.Object);
        Assert.Equal(observer.Object, collection[0]);
    }

    [Fact]
    public void RemovesOnUnregister ()
    {
        var observer = new Mock<IMockObserver>();
        var collection = new List<IMockObserver>();
        var registry = new ObserverRegistry<IMockObserver>(collection);
        registry.Register(observer.Object);
        registry.Unregister(observer.Object);
        Assert.Empty(collection);
    }
}
