using AnkiScraping.Core;
using Microsoft.Extensions.DependencyInjection;

namespace AnkiScraping.WaniKani;

public static class ServiceExtensions
{
    public static IServiceCollection AddWaniKaniScraping(this IServiceCollection services)
    {
        services.AddProvider<IKanjiInformationProvider, WaniKaniKanjiInformationProvider>(WaniKaniConstants.KanjiInformationProviderKey);
        services.AddProvider<IKanjiSetProvider, WaniKaniKanjiSetProvider>(WaniKaniConstants.KanjiSetProviderKey);
        
        services.AddSingleton<KanjiScraper>();
        services.AddSingleton<SetScraper>();
        services.AddSingleton<WaniKaniKanjiMapper>();
        
        return services;
    }
}