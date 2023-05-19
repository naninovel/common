using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;

namespace Naninovel.Observing.Test;

public class RegistryTest
{
    [Fact]
    public void AddsOnRegister ()
    {
        var observer = new Mock<IMockObserver>();
        var registry = new ObserverRegistry<IMockObserver>();
        registry.Register(observer.Object);
        Assert.Equal(observer.Object, registry.Observers.First());
    }

    [Fact]
    public void RemovesOnUnregister ()
    {
        var observer = new Mock<IMockObserver>();
        var registry = new ObserverRegistry<IMockObserver>();
        registry.Register(observer.Object);
        registry.Unregister(observer.Object);
        Assert.Empty(registry.Observers);
    }

    [Fact]
    public void IgnoresDuplicateRegisters ()
    {
        var observer = new Mock<IMockObserver>();
        var registry = new ObserverRegistry<IMockObserver>();
        registry.Register(observer.Object);
        registry.Register(observer.Object);
        Assert.Single(registry.Observers);
    }

    [Fact]
    public void WhenUnregisteringUnknownObserverNothingHappens ()
    {
        var observer = new Mock<IMockObserver>();
        var registry = new ObserverRegistry<IMockObserver>();
        registry.Unregister(observer.Object);
        Assert.Empty(registry.Observers);
    }

    [Fact]
    public void ObserversAreOrderedInRegistrationOrderByDefault ()
    {
        var observer1 = new Mock<IMockObserver>();
        var observer2 = new Mock<IMockObserver>();
        var registry = new ObserverRegistry<IMockObserver>();
        registry.Register(observer1.Object);
        registry.Register(observer2.Object);
        Assert.Equal(observer1.Object, registry.Observers.ElementAtOrDefault(0));
        Assert.Equal(observer2.Object, registry.Observers.ElementAtOrDefault(1));
    }

    [Fact]
    public void SortsAlreadyRegisteredObservers ()
    {
        var observer1 = new Mock<IMockObserver>();
        var observer2 = new Mock<IMockObserver>();
        var registry = new ObserverRegistry<IMockObserver>();
        registry.Register(observer1.Object);
        registry.Register(observer2.Object);
        registry.Order(Comparer<IMockObserver>.Create((x, y) => x == observer1.Object ? 1 : -1));
        Assert.Equal(observer2.Object, registry.Observers.ElementAtOrDefault(0));
        Assert.Equal(observer1.Object, registry.Observers.ElementAtOrDefault(1));
    }

    [Fact]
    public void KeepsOrderAfterRegister ()
    {
        var observer1 = new Mock<IMockObserver>();
        var observer2 = new Mock<IMockObserver>();
        var registry = new ObserverRegistry<IMockObserver>();
        registry.Order(Comparer<IMockObserver>.Create((x, y) => x == observer1.Object ? 1 : -1));
        registry.Register(observer1.Object);
        registry.Register(observer2.Object);
        Assert.Equal(observer2.Object, registry.Observers.ElementAtOrDefault(0));
        Assert.Equal(observer1.Object, registry.Observers.ElementAtOrDefault(1));
    }
}
