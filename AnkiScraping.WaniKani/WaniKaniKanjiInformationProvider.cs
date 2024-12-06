using System.Runtime.CompilerServices;
using AnkiScraping.Core;
using AnkiScraping.Core.Operations;

namespace AnkiScraping.WaniKani;

public class WaniKaniKanjiInformationProvider(
    KanjiScraper scraper,
    WaniKaniKanjiMapper mapper,
    ILogger logger) : IKanjiInformationProvider
{
    private ILogger Logger { get; } =  logger.ForContext<WaniKaniKanjiInformationProvider>();

    public ProviderKey<IKanjiInformationProvider> ProviderKey => WaniKaniConstants.KanjiInformationProviderKey;
    
    public Task<IReadOnlyCollection<KanjiSetId>> ListKanjiSetsAsync(CancellationToken ct = default)
    {
        return Task.FromResult<IReadOnlyCollection<KanjiSetId>>([]);
    }

    public Task<KanjiSet> GetKanjiSetAsync(
        KanjiSetId setId,
        CancellationToken ct = default)
    {
        return Task.FromResult(new KanjiSet());
    }

    public async Task<OneOf<KanjiInformation, KanjiNotFound>> GetKanjiInformationAsync(Kanji kanji, CancellationToken ct = default)
    {
        Logger.Information("Getting information for kanji {Kanji}", kanji);
        
        var scrapeResult = await scraper.ScrapeKanjiInformationAsync(kanji.Character, ct);
        if (!scrapeResult.TryPickT0(out var kanjiInformation, out _))
        {
            Logger.Warning("Kanji {Kanji} not found", kanji);
            return new KanjiNotFound(kanji);
        }
        
        Logger.Information("Found information for kanji {Kanji}", kanji);
        return mapper.Map(kanjiInformation, ProviderKey);
    }

    public async IAsyncEnumerable<OneOf<KanjiInformation, KanjiNotFound>> GetKanjiInformationAsync(
        KanjiSet kanji,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        Logger.Information("Getting information for kanji set {KanjiSet}", kanji);
        
        foreach (var k in kanji)
        {
            yield return await GetKanjiInformationAsync(k, ct);
        }
        
        Logger.Information("Finished getting information for kanji set {KanjiSet}", kanji);
    }
}