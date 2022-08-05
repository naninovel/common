namespace Naninovel.Bindings.Test;

public interface IHandlerSpecifier { }
public interface IHandler<T> where T : IHandlerSpecifier { }
public interface IService { }
public record HandlerSpecifier : IHandlerSpecifier;
public class Service : IService, IHandler<HandlerSpecifier> { }

public interface IRegistrar
{
    void Register<T> (IHandler<T> handler) where T : IHandlerSpecifier;
}
