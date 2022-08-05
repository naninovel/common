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
    public void AddsObserverRegistryAndNotifierForTheProvidedObserver ()
    {
        var provider = new ServiceCollection()
            .AddObserving<IMockObserver>()
            .BuildServiceProvider();
        Assert.NotNull(provider.GetRequiredService<IObserverRegistry<IMockObserver>>());
        Assert.NotNull(provider.GetRequiredService<IObserverNotifier<IMockObserver>>());
    }

    [Fact]
    public async Task RegistersAllObservers ()
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
    public void RegistersImplicitDependencies ()
    {
        var registrar = new Mock<IRegistrar>();
        new ServiceCollection()
            .AddSingleton<IRegistrar>(registrar.Object)
            .AddSingleton<IService, Service>()
            .BuildServiceProvider()
            .Register<IRegistrar, IHandler<HandlerSpecifier>>(c => c.Register);
        registrar.Verify(c => c.Register(It.IsAny<Service>()), Times.Once);
    }
}
