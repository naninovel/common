global using static Naninovel.Bindings.Test.Mocks;
using Naninovel.Observing.Test;

namespace Naninovel.Bindings.Test;

public static class Mocks
{
    public interface IHandlerSpecifier;
    public interface IHandlerOne<T> where T : IHandlerSpecifier;
    public interface IHandlerTwo<T> where T : IHandlerSpecifier;
    public interface IServiceOneA;
    public interface IServiceOneB;
    public interface IServiceOneC;
    public interface IServiceTwoA;
    public interface IServiceTwoB;
    public interface IServiceTwoC;
    public class HandlerSpecifierA : IHandlerSpecifier;
    public class HandlerSpecifierB : IHandlerSpecifier;
    public class ServiceOneA : IServiceOneA, IHandlerOne<HandlerSpecifierA>;
    public class ServiceOneB : IServiceOneB, IHandlerOne<HandlerSpecifierB>;
    public class ServiceOneC : IServiceOneC, IHandlerOne<HandlerSpecifierA>, IHandlerOne<HandlerSpecifierB>;
    public class ServiceTwoA : IServiceTwoA, IHandlerTwo<HandlerSpecifierA>;
    public class ServiceTwoB : IServiceTwoB, IHandlerTwo<HandlerSpecifierB>;
    public class ServiceTwoC : IServiceTwoC, IHandlerTwo<HandlerSpecifierA>, IHandlerTwo<HandlerSpecifierB>;

    public interface IRegistrar
    {
        void Register<T> (IHandlerOne<T> handlerOne) where T : IHandlerSpecifier;
        void Register<T> (IHandlerTwo<T> handlerOne) where T : IHandlerSpecifier;
    }

    public interface IMockGenericObserver<T>;

    public interface IMockNotifier
    {
        void Notify ();
        Task NotifyAsync ();
    }

    public interface IMockObserverService
    {
        bool HandleInvoked { get; }
        bool HandleAsyncInvoked { get; }
    }

    public class MockObserver : IMockObserverService, IMockObserver
    {
        public bool HandleInvoked { get; private set; }
        public bool HandleAsyncInvoked { get; private set; }

        public void Handle () => HandleInvoked = true;

        public Task HandleAsync ()
        {
            HandleAsyncInvoked = true;
            return Task.CompletedTask;
        }
    }

    public class MockNotifier (IObserverNotifier<IMockObserver> notifier) : IMockNotifier
    {
        public void Notify () => notifier.Notify(o => o.Handle());
        public Task NotifyAsync () => notifier.NotifyAsync(o => o.HandleAsync());
    }
}
