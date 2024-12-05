namespace AnkiScraping.Anki;

public interface IAnkiCardCollectionBase
{
    IReadOnlyList<string> Tags { get; }
    int TagCount { get; }
}