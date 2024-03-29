﻿using Bootsharp;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Naninovel.Observing.Test;

[assembly: JSExport([typeof(IServiceOneA)])]
[assembly: JSImport([typeof(IServiceOneB)])]

namespace Naninovel.Bindings.Test;

public class InjectionTest
{
    [Fact]
    public void AddsRegistryAndNotifierForEachObservation ()
    {
        var provider = new ServiceCollection()
            .AddObserving<IMockObserver>()
            .BuildServiceProvider();
        Assert.NotNull(provider.GetRequiredService<IObserverRegistry<IMockObserver>>());
        Assert.NotNull(provider.GetRequiredService<IObserverNotifier<IMockObserver>>());
    }

    [Fact]
    public async Task CanRegisterObservers ()
    {
        var provider = new ServiceCollection()
            .AddObserving<IMockObserver>()
            .AddSingleton<IMockNotifier, MockNotifier>()
            .AddSingleton<IMockObserverService, MockObserver>()
            .BuildServiceProvider()
            .RegisterObservers();
        provider.GetRequiredService<IMockNotifier>().Notify();
        await provider.GetRequiredService<IMockNotifier>().NotifyAsync();
        Assert.True(provider.GetRequiredService<IMockObserverService>().HandleInvoked);
        Assert.True(provider.GetRequiredService<IMockObserverService>().HandleAsyncInvoked);
    }

    [Fact]
    public void CanRegisterHandlers ()
    {
        var registrar = new Mock<IRegistrar>();
        new ServiceCollection()
            .AddSingleton<IRegistrar>(registrar.Object)
            .AddSingleton<IServiceOneA, ServiceOneA>()
            .AddSingleton<IServiceOneB, ServiceOneB>()
            .AddSingleton<IServiceOneC, ServiceOneC>()
            .AddSingleton<IServiceTwoA, ServiceTwoA>()
            .AddSingleton<IServiceTwoB, ServiceTwoB>()
            .AddSingleton<IServiceTwoC, ServiceTwoC>()
            .BuildServiceProvider()
            .Register<IRegistrar, IHandlerOne<HandlerSpecifierA>>(c => c.Register)
            .Register<IRegistrar, IHandlerOne<HandlerSpecifierB>>(c => c.Register)
            .Register<IRegistrar, IHandlerTwo<HandlerSpecifierA>>(c => c.Register)
            .Register<IRegistrar, IHandlerTwo<HandlerSpecifierB>>(c => c.Register);
        registrar.Verify(c => c.Register(It.IsAny<ServiceOneA>()), Times.Once);
        registrar.Verify(c => c.Register(It.IsAny<ServiceOneB>()), Times.Once);
        registrar.Verify(c => c.Register<HandlerSpecifierA>(It.IsAny<ServiceOneC>()), Times.Once);
        registrar.Verify(c => c.Register<HandlerSpecifierB>(It.IsAny<ServiceOneC>()), Times.Once);
        registrar.Verify(c => c.Register(It.IsAny<ServiceTwoA>()), Times.Once);
        registrar.Verify(c => c.Register(It.IsAny<ServiceTwoB>()), Times.Once);
        registrar.Verify(c => c.Register<HandlerSpecifierA>(It.IsAny<ServiceTwoC>()), Times.Once);
        registrar.Verify(c => c.Register<HandlerSpecifierB>(It.IsAny<ServiceTwoC>()), Times.Once);
    }

    [Fact]
    public void CanRegisterGenericHandlers ()
    {
        var registrar = new Mock<IRegistrar>();
        new ServiceCollection()
            .AddSingleton<IRegistrar>(registrar.Object)
            .AddSingleton<IServiceOneA, ServiceOneA>()
            .AddSingleton<IServiceOneB, ServiceOneB>()
            .AddSingleton<IServiceOneC, ServiceOneC>()
            .AddSingleton<IServiceTwoA, ServiceTwoA>()
            .AddSingleton<IServiceTwoB, ServiceTwoB>()
            .AddSingleton<IServiceTwoC, ServiceTwoC>()
            .BuildServiceProvider()
            .Register<IRegistrar>(typeof(IHandlerOne<>))
            .Register<IRegistrar>(typeof(IHandlerTwo<>));
        registrar.Verify(c => c.Register(It.IsAny<ServiceOneA>()), Times.Once);
        registrar.Verify(c => c.Register(It.IsAny<ServiceOneB>()), Times.Once);
        registrar.Verify(c => c.Register<HandlerSpecifierA>(It.IsAny<ServiceOneC>()), Times.Once);
        registrar.Verify(c => c.Register<HandlerSpecifierB>(It.IsAny<ServiceOneC>()), Times.Once);
        registrar.Verify(c => c.Register(It.IsAny<ServiceTwoA>()), Times.Once);
        registrar.Verify(c => c.Register(It.IsAny<ServiceTwoB>()), Times.Once);
        registrar.Verify(c => c.Register<HandlerSpecifierA>(It.IsAny<ServiceTwoC>()), Times.Once);
        registrar.Verify(c => c.Register<HandlerSpecifierB>(It.IsAny<ServiceTwoC>()), Times.Once);
    }
}
