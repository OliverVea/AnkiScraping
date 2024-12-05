using AnkiScraping.Core;
using AnkiScraping.Http;
using AnkiScraping.WaniKani;
using Microsoft.Extensions.DependencyInjection;

namespace AnkiScraping;

public static class ServiceExtensions
{
    public static IServiceCollection AddAnkiScrapingCore(this IServiceCollection services)
    {
        services.AddSingleton<IHttpService, HttpService>();

        services.AddSingleton<IKanjiInformationProvider, WaniKaniKanjiInformationProvider>();
        services.AddSingleton<WaniKaniScraper>();
        services.AddSingleton<WaniKaniKanjiMapper>();
        services.AddSingleton<DebugJsonOptions>();
        
        return services;
    }
}