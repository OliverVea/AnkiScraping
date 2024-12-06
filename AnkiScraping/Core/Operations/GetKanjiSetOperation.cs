namespace AnkiScraping.Core.Operations;

using Request = GetKanjiSetOperation.Request;
using Response = OneOf<KanjiSet, ProviderNotFound<IKanjiSetProvider>, KanjiSetNotFound>;

public class GetKanjiSetOperation(ProviderFactory providerFactory, ILogger logger) : IAsyncOperation<Request, Response>
{
    private ILogger Logger => logger.ForContext<GetKanjiSetOperation>();
    
    public class Request
    {
        public KanjiSetId KanjiSetId { get; init; } = new();
    }
    
    public async Task<Response> ExecuteAsync(Request request, CancellationToken ct = default)
    {
        var providerQuery = new ProviderQuery<IKanjiSetProvider>
        {
            ProviderIdentifier = request.KanjiSetId.ProviderKey.ProviderIdentifier
        };

        var providerResult = providerFactory.GetProvider(providerQuery);
        if (!providerResult.TryPickT0(out var kanjiSetProvider, out var providerNotFound))
        {
            Logger.Warning("Provider not found for {ProviderQuery}", providerQuery);
            return providerNotFound;
        }
        
        var kanjiSetResult = await kanjiSetProvider.GetKanjiSetAsync(request.KanjiSetId, ct);
        if (!kanjiSetResult.TryPickT0(out var kanjiSet, out var kanjiSetNotFound))
        {
            Logger.Warning("Kanji set not found for {KanjiSetId}", request.KanjiSetId);
            return kanjiSetNotFound;
        }

        return kanjiSet;
    }
}