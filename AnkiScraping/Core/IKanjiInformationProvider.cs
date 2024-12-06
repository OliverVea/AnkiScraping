using AnkiScraping.Core.Operations;

namespace AnkiScraping.Core;

public interface IKanjiInformationProvider
{
    ProviderKey<IKanjiInformationProvider> ProviderKey { get; }
    Task<OneOf<KanjiInformation, KanjiNotFound>> GetKanjiInformationAsync(Kanji kanji, CancellationToken ct = default);
    IAsyncEnumerable<OneOf<KanjiInformation, KanjiNotFound>> GetKanjiInformationAsync(KanjiSet kanji, CancellationToken ct = default);
}