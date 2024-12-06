namespace AnkiScraping.Core.Operations;

using Response = OneOf<IReadOnlyCollection<KanjiSetId>, ProviderNotFound<IKanjiSetProvider>>;

public class ListKanjiSetsOperation(
    ProviderFactory providerFactory,
    ILogger logger)
    : IAsyncOperation<ListKanjiSetsOperation.Request, Response>
{
    private ILogger Logger => logger.ForContext<ListKanjiSetsOperation>();
    
    public class Request
    {
        public ProviderQuery<IKanjiSetProvider> ProviderQuery { get; init; }
    }

    public async Task<Response> ExecuteAsync(Request request, CancellationToken ct = default)
    {
        Logger.Information("Listing kanji sets from provider {ProviderId}", request.ProviderQuery);
        if (!providerFactory.GetProvider(request.ProviderQuery).TryPickT0(out var kanjiSetProvider, out var providerNotFound))
        {
            Logger.Error("Kanji set provider not found");
            
            return providerNotFound;
        }
        
        var kanjiSets = await kanjiSetProvider.ListKanjiSetsAsync(ct);
        
        Logger.Information("Found {KanjiSetCount} kanji sets from provider {ProviderId}", kanjiSets.Count, request.ProviderQuery);
        
        return Response.FromT0(kanjiSets);
    }
}