using AnkiScraping.Core;

namespace AnkiScraping.WaniKani;

public class WaniKaniConstants
{
    public const string ProviderIdentifier = "wk";
    
    public static readonly ProviderKey<IKanjiSetProvider> KanjiSetProviderKey = new(ProviderIdentifier);
    public static readonly ProviderKey<IKanjiInformationProvider> KanjiInformationProviderKey = new(ProviderIdentifier);
}