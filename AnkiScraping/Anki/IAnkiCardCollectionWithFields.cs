namespace AnkiScraping.Anki;

public interface IAnkiCardCollectionWithFields : IAnkiCardCollectionBase
{
    IReadOnlyList<string> Fields { get; }
    
    int FieldCount { get; }
    AnkiCardTextFileSeparator DefaultSeparator { get; }
    IAnkiCardCollectionWithFields WithDefaultSeparator(AnkiCardTextFileSeparator separator);
    
    IAnkiCardCollectionWithFields AddCard(params IReadOnlyCollection<string> fieldValues);
    IAnkiCardCollectionWithFields AddCards(params IReadOnlyCollection<IReadOnlyCollection<string>> cards);

    string ExportToString(AnkiCardTextFileSeparator? separator = null);
    Task<ExportResult> ExportToCsvAsync(string filePath, AnkiCardTextFileSeparator? separator = null, CancellationToken ct = default);
    Task<ExportResult> ExportToAnkiPackageAsync(string filePath, CancellationToken ct = default);
}