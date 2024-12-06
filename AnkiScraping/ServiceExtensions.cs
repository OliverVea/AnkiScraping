using System.Diagnostics.CodeAnalysis;
using AnkiScraping.Anki;
using AnkiScraping.Core;
using AnkiScraping.Core.Operations;
using AnkiScraping.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AnkiScraping;

public static class ServiceExtensions
{
    public static IServiceCollection AddAnkiScrapingCore(this IServiceCollection services)
    {
        services.AddSingleton<IHttpService, HttpService>();

        services.AddSingleton<ProviderFactory>();
        
        services.AddSingleton<GetMultipleKanjiInformationAsyncOperation>();
        services.AddSingleton<GetKanjiSetOperation>();
        services.AddSingleton<ListKanjiSetsOperation>();
        services.AddSingleton<GetAnkiCardCollectionOperation>();
        
        return services;
    }
    
    public static IServiceCollection AddProvider<TInterface,[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImpl>(
        this IServiceCollection services,
        ProviderKey<TInterface> providerKey)
            where TImpl : class, TInterface
            where TInterface : class
    {
        services.AddSingleton<TInterface, TImpl>();
        services.AddKeyedSingleton(providerKey.ProviderIdentifier, KeyedFactory);
        
        return services;

        TInterface KeyedFactory(IServiceProvider sp, object? id) => sp.GetRequiredService<TInterface>();
    }
    
    public static IServiceCollection AddProvider<T>(
        this IServiceCollection services,
        ProviderKey<IKanjiInformationProvider> providerKey,
        Func<IServiceProvider, T> factory)
        where T : class, IKanjiInformationProvider
    {
        services.AddSingleton<IKanjiInformationProvider>(factory);
        services.AddKeyedSingleton<IKanjiInformationProvider, T>(providerKey, KeyedFactory);
        
        return services;

        T KeyedFactory(IServiceProvider sp, object? id) => factory(sp);
    }
}