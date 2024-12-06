using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using AnkiScraping.Core;
using AnkiScraping.Core.Operations;
using Spectre.Console;
using Spectre.Console.Cli;

namespace AnkiScraping.Host.CLI;

public partial class ScrapeCommand : AsyncCommand<ScrapeCommand.Settings>
{
    public const int ProviderNotFoundExitCode = 1;
    
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        if (settings.Verbose)
        {
            console.MarkupLine($"Scraping information for kanji characters: [bold]{settings.KanjiList}[/]");
        }

        var allKanji = await GetKanjiSetAsync(settings);

        var kanjiRequest = new GetMultipleKanjiInformationAsyncOperation.Request(allKanji);
        var kanjiResult = getMultipleKanjiInformationAsyncOperation.Execute(kanjiRequest);
        if (!kanjiResult.TryPickT0(out var kanjiEnumerable, out var providerNotFound))
        {
            console.MarkupErrorLine($"No providers found for kanji characters: {providerNotFound}");
            return ProviderNotFoundExitCode;
        }
        
        var filteredKanjiInformation = FilterNotFoundKanji(kanjiEnumerable, settings, CancellationToken);
        
        var cardCollection = await getAnkiCardCollectionOperation.ExecuteAsync(filteredKanjiInformation, CancellationToken);

        if (settings.DryRun)
        {
            var text = cardCollection.ExportToString();
            console.MarkupLine($"[bold]Dry run output:[/]\n{text}");
        }
        else
        {
            await cardCollection.ExportToCsvAsync(settings.OutputFile, ct: CancellationToken);
        }
        
        return 0;
    }

    private async Task<KanjiSet> GetKanjiSetAsync(Settings settings)
    {
        var singleKanji = settings.SingleKanji.Select(x => new Kanji(x[0])).ToArray();
        var kanjiFromList = settings.KanjiList?.Select(x => new Kanji(x)).ToArray() ?? [];
        var kanjiFromSets = await GetKanjiFromSetsAsync(settings);
        
        Logger.Information("Getting information for {KanjiCount} kanji characters (single: {SingleKanjiCount}, list: {ListKanjiCount}, sets: {SetKanjiCount})",
            singleKanji.Length + kanjiFromList.Length + kanjiFromSets.Count,
            singleKanji.Length,
            kanjiFromList.Length,
            kanjiFromSets.Count);
        
        return new KanjiSet(singleKanji.Concat(kanjiFromList).Concat(kanjiFromSets).ToArray());
    }

    private async Task<List<Kanji>> GetKanjiFromSetsAsync(Settings settings)
    {
        var kanji = new List<Kanji>();
        var notFoundQueue = new ConcurrentQueue<(OneOf.OneOf<ProviderNotFound<IKanjiSetProvider>, KanjiSetNotFound>, KanjiSetId)>();
        
        foreach (var kanjiSetIdString in settings.KanjiSets)
        {
            var kanjiSetId = ParseKanjiSetId(kanjiSetIdString);
            
            var request = new GetKanjiSetOperation.Request { KanjiSetId = kanjiSetId };
            
            var result = await getKanjiSetOperation.ExecuteAsync(request, CancellationToken);
            
            if (!result.TryPickT0(out var kanjiSet, out var notFound))
            {
                notFoundQueue.Enqueue((notFound, kanjiSetId));
                continue;
            }
            
            kanji.AddRange(kanjiSet.Kanji);
        }
        
        if (notFoundQueue.Count > 0)
        {
            foreach (var notFound in notFoundQueue)
            {
                notFound.Item1.Switch(
                    providerNotFound => console.MarkupErrorLine($"Provider not found for {providerNotFound.Query}. Ignoring {notFound.Item2}..."),
                    kanjiSetNotFound => console.MarkupErrorLine($"Kanji set not found for {kanjiSetNotFound.KanjiSetId}. Ignoring {notFound.Item2}...")
                );
            }
        }
        
        return kanji;
    }
    
    private static KanjiSetId ParseKanjiSetId(string kanjiSetIdString)
    {
        var parts = kanjiSetIdString.Split(KanjiSetId.Separator);
        
        var providerKey = new ProviderKey<IKanjiSetProvider>(parts[0]);
        var setIdentifier = parts[1];
        
        return new KanjiSetId(providerKey, setIdentifier);
    }
    
    private async IAsyncEnumerable<KanjiInformation> FilterNotFoundKanji(
        IAsyncEnumerable<OneOf.OneOf<KanjiInformation, KanjiNotFound>> kanjiInformation,
        Settings settings,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var kanjiInfo in kanjiInformation.WithCancellation(ct))
        {
            if (kanjiInfo.IsT0)
            {
                yield return kanjiInfo.AsT0;
            }

            else
            {
                if (settings.Verbose)
                {
                    console.MarkupLine($"Kanji {kanjiInfo.AsT1.Kanji} not found in any provider");
                }
            }
        }
    }
}