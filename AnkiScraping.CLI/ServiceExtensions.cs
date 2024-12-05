using AnkiScraping.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Spectre.Console;

namespace AnkiScraping.Host;

public static class ServiceExtensions
{
    public static IServiceCollection RegisterCliServices(this IServiceCollection services)
    {
#pragma warning disable EXTEXP0018
        services.AddHybridCache();
#pragma warning restore EXTEXP0018
        
        services.AddLogging();
        
        services.AddAnkiScrapingCore();
        services.AddSqliteIDistributedCache();

        services.AddHttpClient();
        
        var cancellationTokenSource = new CancellationTokenSource();
        services.AddSingleton(cancellationTokenSource);
        
        services.TryAddSingleton(BuildConsole);
        
        return services;
    }

    private static IAnsiConsole BuildConsole(IServiceProvider serviceProvider)
    {
        var settings = serviceProvider.GetService<AnsiConsoleSettings>() ?? new AnsiConsoleSettings();
        
        return AnsiConsole.Create(settings);
    }
}