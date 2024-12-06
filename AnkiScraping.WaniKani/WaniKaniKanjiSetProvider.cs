using AnkiScraping.Core;
using OneOf.Types;

namespace AnkiScraping.WaniKani;

public class WaniKaniKanjiSetProvider(SetScraper setScraper) : IKanjiSetProvider
{
    public ProviderKey<IKanjiSetProvider> ProviderKey => WaniKaniConstants.KanjiSetProviderKey;

    private const int Levels = 60;
    
    public Task<IReadOnlyCollection<KanjiSetId>> ListKanjiSetsAsync(CancellationToken ct = default)
    {
        var kanjiSets = ListKanjiSets().ToList();
        
        return Task.FromResult<IReadOnlyCollection<KanjiSetId>>(kanjiSets);
    }
    
    private IEnumerable<KanjiSetId> ListKanjiSets()
    {
        for (var i = 1; i <= Levels; i++)
        {
            yield return GetKanjiSetId(i);
        }
    }
    
    private KanjiSetId GetKanjiSetId(int level)
    {
        return new KanjiSetId(ProviderKey, level.ToString());
    }

    public async Task<OneOf<KanjiSet, KanjiSetNotFound>> GetKanjiSetAsync(KanjiSetId setId, CancellationToken ct = default)
    {
        if (!ParseLevel(setId).TryPickT0(out var level, out _))
        {
            return new KanjiSetNotFound(setId);
        }
        
        var kanjiSet = await setScraper.ScrapeKanjiForLevelAsync(level, ct);

        return new KanjiSet
        {
            Kanji = kanjiSet.Select(x => new Kanji(x)).ToArray().AsReadOnly()
        };
    }
    
    private OneOf<int, Error> ParseLevel(KanjiSetId setId)
    {
        if (!int.TryParse(setId.SetIdentifier, out var level))
        {
            return new Error();
        }
        
        return level;
    }
}