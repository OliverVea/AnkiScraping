namespace AnkiScraping.Anki;

public static class AnkiCardCollectionBuilder
{
    public static IAnkiCardCollectionWithoutFields Create()
    {
        return new AnkiCardCollection();
    }
}