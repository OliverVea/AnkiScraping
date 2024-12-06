namespace AnkiScraping.Core;

public interface IKanjiSetProvider
{
    ProviderKey<IKanjiSetProvider> ProviderKey { get; }
    Task<IReadOnlyCollection<KanjiSetId>> ListKanjiSetsAsync(CancellationToken ct = default);
    Task<OneOf<KanjiSet, KanjiSetNotFound>> GetKanjiSetAsync(KanjiSetId setId, CancellationToken ct = default);
}