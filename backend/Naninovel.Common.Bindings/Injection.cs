using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Naninovel.Bindings;

[UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "False-positive?")]
public static class Injection
{
    private const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(ServiceProvider))]
    private static readonly PropertyInfo callSiteProperty = typeof(ServiceProvider).GetProperty("CallSiteFactory", flags)!;
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicFields, "Microsoft.Extensions.DependencyInjection.ServiceLookup.CallSiteFactory", "Microsoft.Extensions.DependencyInjection")]
    private static readonly FieldInfo descriptorsField = callSiteProperty.PropertyType.GetField("_descriptors", flags)!;

    public static IEnumerable<T> GetAll<T> (this IServiceProvider provider)
    {
        return GetAll(provider).OfType<T>();
    }

    public static IEnumerable<object> GetAll (this IServiceProvider provider, Predicate<Type>? predicate = null)
    {
        var site = callSiteProperty.GetValue(provider);
        var desc = descriptorsField.GetValue(site) as ServiceDescriptor[];
        var types = desc!.Select(s => s.ServiceType);
        if (predicate != null) types = types.Where(predicate.Invoke);
        return types.Select(provider.GetRequiredService);
    }

    public static IServiceCollection AddObserving<TObserver> (this IServiceCollection services)
    {
        var registry = new ObserverRegistry<TObserver>();
        services.AddSingleton<IObserverRegistry<TObserver>>(registry);
        services.AddSingleton<IObserverNotifier<TObserver>>(new ObserverNotifier<TObserver>(registry));
        return services;
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicMethods, typeof(ObserverRegistry<>))]
    public static IServiceProvider RegisterObservers (this IServiceProvider provider)
    {
        foreach (var registry in provider.GetAll(IsObserverRegistry))
        foreach (var observer in provider.GetAll().Where(s => CanBeRegistered(s, registry)))
            registry.GetType().GetMethod("Register")!.Invoke(registry, [observer]);
        return provider;

        bool IsObserverRegistry (Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IObserverRegistry<>);
        bool CanBeRegistered (object service, object registry) => registry.GetType().GetGenericArguments()[0].IsInstanceOfType(service);
    }

    public static IServiceProvider Register<TRegistrar, THandler> (this IServiceProvider provider,
        Func<TRegistrar, Action<THandler>> resolve) where TRegistrar : notnull
    {
        var registrar = provider.GetRequiredService<TRegistrar>();
        foreach (var handler in provider.GetAll<THandler>())
            resolve(registrar)(handler);
        return provider;
    }

    public static IServiceProvider Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] TRegistrar> (
        this IServiceProvider provider, Type genericHandler, string registerMethodName = "Register", int specifierIndex = 0) where TRegistrar : notnull
    {
        var registerMethod = typeof(TRegistrar).GetMethods().First(m =>
            m.Name == registerMethodName &&
            m.GetParameters().Length > specifierIndex &&
            m.GetParameters()[specifierIndex].ParameterType.GetGenericTypeDefinition() == genericHandler);
        var registrar = provider.GetRequiredService<TRegistrar>();
        foreach (var service in provider.GetAll())
        foreach (var handler in GetHandlerTypes(service))
            RegisterHandler(service, handler);
        return provider;

        Type[] GetHandlerTypes (object service) => service.GetType().GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericHandler).ToArray();

        [UnconditionalSuppressMessage("Trimming", "IL2060", Justification = "MakeGenericMethod() is not analyzable.")]
        void RegisterHandler (object handler, Type handlerType)
        {
            var specifier = handlerType.GetGenericArguments()[specifierIndex];
            var method = registerMethod.MakeGenericMethod(specifier);
            method.Invoke(registrar, [handler]);
        }
    }
}
