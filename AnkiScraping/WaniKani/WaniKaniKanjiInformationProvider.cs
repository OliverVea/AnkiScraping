using AnkiScraping.Core;

namespace AnkiScraping.WaniKani;

public class WaniKaniKanjiInformationProvider(
    WaniKaniScraper scraper,
    WaniKaniKanjiMapper mapper) : IKanjiInformationProvider
{
    public async Task<KanjiCardInformation> GetKanjiInformationAsync(char kanji, CancellationToken ct = default)
    {
        var scrapedKanji = await scraper.ScrapeKanjiInformationAsync(kanji, ct);
        
        return mapper.Map(scrapedKanji);
    }
}