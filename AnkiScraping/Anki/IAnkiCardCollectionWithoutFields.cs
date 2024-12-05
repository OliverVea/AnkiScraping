namespace AnkiScraping.Anki;

public interface IAnkiCardCollectionWithoutFields : IAnkiCardCollectionBase
{
    IAnkiCardCollectionWithoutFields AddTags(params string[] tags);
    IAnkiCardCollectionWithoutFields WithTags(params string[] tags);
    
    IAnkiCardCollectionWithFields WithFields(params string[] fields);
}