using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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
            .AddSingleton<MockNotifier>()
            .AddSingleton<MockObserver>()
            .BuildServiceProvider()
            .RegisterObservers();
        provider.GetRequiredService<MockNotifier>().Notify();
        await provider.GetRequiredService<MockNotifier>().NotifyAsync();
        Assert.True(provider.GetRequiredService<MockObserver>().HandleInvoked);
        Assert.True(provider.GetRequiredService<MockObserver>().HandleAsyncInvoked);
    }
}
