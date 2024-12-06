namespace AnkiScraping.Core;

public readonly record struct Kanji(char Character)
{
    public override string ToString()
    {
        return Character.ToString();
    }
}