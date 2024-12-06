using Microsoft.Extensions.DependencyInjection;

namespace AnkiScraping.Core;

public class ProviderFactory(IServiceProvider serviceProvider, ILogger logger)
{
    private ILogger Logger => logger.ForContext<ProviderFactory>();
    
    public OneOf<T, ProviderNotFound<T>> GetProvider<T>(ProviderQuery<T> providerQuery)
    {
        Logger.Information("Getting provider for {ProviderQuery}", providerQuery);
        
        var provider = providerQuery.ProviderIdentifier.TryPickT0(out var providerKey, out _)
            ? serviceProvider.GetKeyedService<T>(providerKey)
            : serviceProvider.GetService<T>();

        if (provider is null)
        {
            Logger.Error("Provider not found for {ProviderQuery}", providerQuery);
            
            return new ProviderNotFound<T>(providerQuery);
        }
        
        Logger.Information("Resolved provider for {ProviderQuery} to {Provider}", providerQuery, provider);
        
        return OneOf<T, ProviderNotFound<T>>.FromT0(provider);
    }   
}