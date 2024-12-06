using System.Diagnostics.CodeAnalysis;
using System.Text;
using AnkiScraping.Host.CLI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace AnkiScraping.Host;

public class Program
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ScrapeCommand))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ScrapeCommand.Settings))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, "Spectre.Console.Cli.ExplainCommand", "Spectre.Console.Cli")]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, "Spectre.Console.Cli.VersionCommand", "Spectre.Console.Cli")]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, "Spectre.Console.Cli.XmlDocCommand", "Spectre.Console.Cli")]
    public static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = Encoding.Unicode;
        
        var configuration = new ConfigurationBuilder()
            .AddIniFile("config.ini", optional: true)
            .AddEnvironmentVariables("ANKI_SCRAPING_")
            .AddUserSecrets<Program>(optional: true)
            .Build();
        
        var serviceCollection = new ServiceCollection();
        serviceCollection.RegisterCliServices(configuration);

        var registrar = new TypeRegistrar(serviceCollection);
        var commandApp = new CommandApp(registrar);

        serviceCollection.AddSingleton<ScrapeCommand>();

        serviceCollection.AddSingleton<ScrapeCommand>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var scrapeCommand = serviceProvider.GetRequiredService<ScrapeCommand>();
        
        commandApp.Configure(config =>
        {
            ScrapeCommand.AddToConfigurator(config);
            ListKanjiSetsCommand.AddToConfigurator(config);
        });
        
        return await commandApp.RunAsync(args);
    }
}