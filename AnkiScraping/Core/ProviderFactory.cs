using AnkiScraping.Core.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace AnkiScraping.Core;

public class KanjiProviderFactory(IServiceProvider serviceProvider)
{
    public OneOf<IKanjiInformationProvider, KanjiProviderNotFound> GetProvider(KanjiProviderKeyOrAny providerId)
    {
        var provider = providerId.TryPickT0(out var providerKey, out _)
            ? serviceProvider.GetKeyedService<IKanjiInformationProvider>(providerKey)
            : serviceProvider.GetService<IKanjiInformationProvider>();
        
        return provider is null 
            ? new KanjiProviderNotFound(providerId)
            : OneOf<IKanjiInformationProvider, KanjiProviderNotFound>.FromT0(provider);
    }   
}

public readonly record struct KanjiProviderNotFound(KanjiProviderKeyOrAny ProviderId);