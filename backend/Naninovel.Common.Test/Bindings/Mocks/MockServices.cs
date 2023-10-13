namespace Naninovel.Bindings.Test;

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
