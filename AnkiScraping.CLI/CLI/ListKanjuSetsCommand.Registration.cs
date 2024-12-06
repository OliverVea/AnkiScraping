using Spectre.Console.Cli;

namespace AnkiScraping.Host.CLI;

public partial class ListKanjiSetsCommand
{
    private const string Name = "list-kanji-sets";
    private const string Description = "List all kanji sets.";
    
    private static readonly string[] Examples =
    [
        "list-kanji-sets"
    ];
        
    public static void AddToConfigurator(IConfigurator config)
    {
        config.AddCommand<ListKanjiSetsCommand, Settings>(Name, Description, Examples);
    }    
}