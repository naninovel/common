namespace Naninovel.Bindings.Test;

public interface IHandlerSpecifier { }
public interface IHandler<T> where T : IHandlerSpecifier { }
public interface IServiceA { }
public interface IServiceB { }
public record HandlerSpecifierA : IHandlerSpecifier;
public record HandlerSpecifierB : IHandlerSpecifier;
public class ServiceA : IServiceA, IHandler<HandlerSpecifierA> { }
public class ServiceB : IServiceB, IHandler<HandlerSpecifierB> { }

public interface IRegistrar
{
    void Register<T> (IHandler<T> handler) where T : IHandlerSpecifier;
}
