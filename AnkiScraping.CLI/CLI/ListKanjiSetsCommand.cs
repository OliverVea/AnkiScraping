using AnkiScraping.Core.Operations;
using Spectre.Console;
using Spectre.Console.Cli;

namespace AnkiScraping.Host.CLI;

public partial class ListKanjiSetsCommand(
    ListKanjiSetsOperation listKanjiSetsOperation,
    IAnsiConsole console,
    CancellationTokenSource cancellationTokenSource) : AsyncCommand<ListKanjiSetsCommand.Settings>
{
    private CancellationToken CancellationToken => cancellationTokenSource.Token;
    
    private const int ProviderNotFoundExitCode = 1;

    public class Settings : CommandSettings;


    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var request = new ListKanjiSetsOperation.Request();
        
        var kanjiSetsResult = await listKanjiSetsOperation.ExecuteAsync(request, CancellationToken);
        if (!kanjiSetsResult.TryPickT0(out var kanjiSetIds, out var providerNotFound))
        {
            console.MarkupErrorLine($"No providers found for kanji sets: {providerNotFound}");
            return ProviderNotFoundExitCode;
        }
        
        console.MarkupLine("Kanji sets:");
        
        foreach (var kanjiSetId in kanjiSetIds)
        {
            console.MarkupLine($"[bold]{kanjiSetId}[/]");
        }
        
        return 0;
    }
}

