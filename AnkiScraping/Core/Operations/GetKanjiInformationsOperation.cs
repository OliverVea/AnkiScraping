using Request = AnkiScraping.Core.Operations.GetMultipleKanjiInformationAsyncOperation.Request;
using Response = OneOf.OneOf<System.Collections.Generic.IAsyncEnumerable<OneOf.OneOf<AnkiScraping.Core.KanjiInformation, AnkiScraping.KanjiNotFound>>, AnkiScraping.Core.ProviderNotFound<AnkiScraping.Core.IKanjiInformationProvider>>;

namespace AnkiScraping.Core.Operations;

public class GetMultipleKanjiInformationAsyncOperation(
    ProviderFactory providerFactory,
    ILogger logger)
    : IOperation<Request, Response>
{
    private ILogger Logger => logger.ForContext<GetMultipleKanjiInformationAsyncOperation>();
    
    public readonly record struct Request(KanjiSet KanjiSet, CancellationToken CancellationToken = default)
    {
        public ProviderQuery<IKanjiInformationProvider> ProviderQuery { get; init; } = new();
    }

    public Response Execute(Request request)
    {
        Logger.Information("Getting information for kanji set {KanjiSet} from provider {ProviderId}", request.KanjiSet, request.ProviderQuery);
        
        var getKanjiProviderResult = providerFactory.GetProvider(request.ProviderQuery);
        if (!getKanjiProviderResult.TryPickT0(out var kanjiProvider, out var providerNotFound))
        {
            Logger.Error("Kanji provider not found");
            
            return providerNotFound;
        }
        
        Logger.Information("Resolved provider {ProviderId} to provider with key {ProviderKey}", request.ProviderQuery, kanjiProvider.ProviderKey);
        
        var result = kanjiProvider.GetKanjiInformationAsync(request.KanjiSet, request.CancellationToken);
        var filteredResult = FilterResult(result, kanjiProvider.ProviderKey);
        
        return Response.FromT0(filteredResult);
    }

    private async IAsyncEnumerable<OneOf<KanjiInformation, KanjiNotFound>> FilterResult(
        IAsyncEnumerable<OneOf<KanjiInformation, KanjiNotFound>> result,
        ProviderKey<IKanjiInformationProvider> providerKey)
    {
        await foreach (var kanjiInformation in result)
        {
            if (kanjiInformation.TryPickT0(out var kanjiInfo, out var kanjiNotFound))
            {
                Logger.Debug("Found kanji information {KanjiInfo} in provider with key {ProviderId}", kanjiInfo, providerKey);
                yield return kanjiInfo;
            }
            else
            {
                Logger.Warning("Kanji {Kanji} not found in provider with key {ProviderId}", kanjiNotFound.Kanji, providerKey);
                yield return kanjiNotFound;
            }
        }
        
        Logger.Information("Finished filtering results for provider with key {ProviderId}", providerKey);
    }
}