﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Naninovel.Common.Bindings;

public static class Injection
{
    private const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
    private static readonly PropertyInfo callSiteProperty = typeof(ServiceProvider).GetProperty("CallSiteFactory", flags)!;
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
        var observers = new HashSet<TObserver>();
        services.AddSingleton<IObserverRegistry<TObserver>>(new ObserverRegistry<TObserver>(observers));
        services.AddSingleton<IObserverNotifier<TObserver>>(new ObserverNotifier<TObserver>(observers));
        return services;
    }

    public static IServiceProvider RegisterObservers (this IServiceProvider provider)
    {
        foreach (var registry in provider.GetAll(IsObserverRegistry))
        foreach (var observer in provider.GetAll().Where(s => CanBeRegistered(s, registry)))
            registry.GetType().GetMethod("Register")!.Invoke(registry, new[] { observer });
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
}