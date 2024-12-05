namespace AnkiScraping.Core;

public interface IKanjiInformationProvider
{
    Task<KanjiCardInformation> GetKanjiInformationAsync(char kanji, CancellationToken ct = default);
}