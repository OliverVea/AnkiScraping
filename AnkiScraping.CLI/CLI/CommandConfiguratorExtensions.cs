using Spectre.Console.Cli;

namespace AnkiScraping.Host.CLI;

public static class CommandConfiguratorExtensions
{
    public static IConfigurator AddCommand<T, TSettings>(this IConfigurator config, string name, string description, string[] examples) 
        where T : class, ICommand<TSettings>
        where TSettings : CommandSettings
    {
        var command = config.AddCommand<T>(name);
            
        command.WithDescription(description);

        foreach (var example in examples)
        {
            command.WithExample(example);
        }
        
        return config;
    }
}