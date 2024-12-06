namespace AnkiScraping.Core;

public readonly record struct KanjiProviderKey(string Value)
{
    public override string ToString()
    {
        return Value;
    }
}