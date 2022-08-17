using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Naninovel.Common.Bindings;
using Naninovel.Observing.Test;
using Xunit;

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
            .AddSingleton<IServiceA, ServiceA>()
            .AddSingleton<IServiceB, ServiceB>()
            .AddSingleton<IServiceC, ServiceC>()
            .BuildServiceProvider()
            .Register<IRegistrar, IHandler<HandlerSpecifierA>>(c => c.Register)
            .Register<IRegistrar, IHandler<HandlerSpecifierB>>(c => c.Register);
        registrar.Verify(c => c.Register(It.IsAny<ServiceA>()), Times.Once);
        registrar.Verify(c => c.Register(It.IsAny<ServiceB>()), Times.Once);
        registrar.Verify(c => c.Register<HandlerSpecifierA>(It.IsAny<ServiceC>()), Times.Once);
        registrar.Verify(c => c.Register<HandlerSpecifierB>(It.IsAny<ServiceC>()), Times.Once);
    }

    [Fact]
    public void CanRegisterGenericHandlers ()
    {
        var registrar = new Mock<IRegistrar>();
        new ServiceCollection()
            .AddSingleton<IRegistrar>(registrar.Object)
            .AddSingleton<IServiceA, ServiceA>()
            .AddSingleton<IServiceB, ServiceB>()
            .AddSingleton<IServiceC, ServiceC>()
            .BuildServiceProvider()
            .Register<IRegistrar>(typeof(IHandler<>));
        registrar.Verify(c => c.Register(It.IsAny<ServiceA>()), Times.Once);
        registrar.Verify(c => c.Register(It.IsAny<ServiceB>()), Times.Once);
        registrar.Verify(c => c.Register<HandlerSpecifierA>(It.IsAny<ServiceC>()), Times.Once);
        registrar.Verify(c => c.Register<HandlerSpecifierB>(It.IsAny<ServiceC>()), Times.Once);
    }
}
