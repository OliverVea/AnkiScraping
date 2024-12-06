namespace AnkiScraping.Core;

public readonly record struct KanjiSetId(ProviderKey<IKanjiSetProvider> ProviderKey, string SetIdentifier)
{
    public const char Separator = '-';
    
    public override string ToString()
    {
        return $"{ProviderKey.ProviderIdentifier}{Separator}{SetIdentifier}";
    }
}