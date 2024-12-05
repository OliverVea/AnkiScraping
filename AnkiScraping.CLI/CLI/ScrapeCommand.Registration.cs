using Spectre.Console.Cli;

namespace AnkiScraping.Host.CLI;

public partial class ScrapeCommand
{
    private const string Name = "scrape";
    private const string Description = "Scrape information for kanji characters and output it to a file.";

    private static readonly string[] Examples =
    [
        "scrape -k 人",
        "scrape -k 人 -k 木",
        "scrape -l 人木"
    ];
    
    public static void AddToConfigurator(IConfigurator config)
    {
        config.AddCommand<ScrapeCommand, Settings>(Name, Description, Examples);
    }
}