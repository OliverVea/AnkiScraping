using AnkiScraping.Caching;
using AnkiScraping.WaniKani;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Spectre.Console;

namespace AnkiScraping.Host;

public static class ServiceExtensions
{
    public static IServiceCollection RegisterCliServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        
#pragma warning disable EXTEXP0018
        services.AddHybridCache();
#pragma warning restore EXTEXP0018
        
        var logger = BuildLogger(configuration);
        if (logger != null)
        {
            services.AddSingleton<ILogger>(logger);
        }
        
        services.AddLogging(loggingBuilder =>
        {
            if (logger != null)
            {
                loggingBuilder.AddSerilog(logger);
            }
        });
        
        services.AddWaniKaniScraping();
        
        services.AddAnkiScrapingCore();
        services.AddSqliteIDistributedCache(configuration);

        services.AddHttpClient();
        
        var cancellationTokenSource = new CancellationTokenSource();
        services.AddSingleton(cancellationTokenSource);
        
        services.TryAddSingleton(BuildConsole);
        
        return services;
    }

    private static Logger? BuildLogger(IConfiguration configuration)
    {
        var loggingConfiguration = configuration.GetSection(LoggingConfiguration.Section).Get<LoggingConfiguration>();
        
        if (loggingConfiguration == null) return null;
            
        var loggerQueue = new List<Action<Logger>>();
            
        if (!Enum.TryParse<LogEventLevel>(loggingConfiguration.LogLevel, out var logLevel))
        {
            var allowedValues = string.Join(", ", Enum.GetNames<LogEventLevel>());
            loggerQueue.Add(logger => logger.Warning("Invalid log level {LogLevel}. Allowed values: {AllowedValues}. Defaulting to Information.", loggingConfiguration.LogLevel, allowedValues));
            logLevel = LogEventLevel.Information;
        }

        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel);
            
        if (loggingConfiguration.LogFile is { } logFile)
        {
            loggerConfiguration.WriteTo.File(logFile);
        }
            
        if (loggingConfiguration.LogToConsole)
        {
            const string outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}";
            loggerConfiguration.WriteTo.Console(outputTemplate: outputTemplate);
        }
            
        var logger = loggerConfiguration.CreateLogger();
            
        foreach (var action in loggerQueue)
        {
            action(logger);
        }

        return logger;
    }

    private static IAnsiConsole BuildConsole(IServiceProvider serviceProvider)
    {
        var settings = serviceProvider.GetService<AnsiConsoleSettings>() ?? new AnsiConsoleSettings();
        
        return AnsiConsole.Create(settings);
    }
}