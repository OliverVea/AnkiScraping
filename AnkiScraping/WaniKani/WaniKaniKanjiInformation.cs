namespace AnkiScraping.WaniKani;

public class WaniKaniKanjiInformation
{
    public required char Kanji { get; set; }
    public string? HeaderText { get; set; }
    public WaniKaniRadicalInformation[]? Radicals { get; set; }
    public string[]? Meanings { get; set; }
    public string? KanjiMnemonic { get; set; }
    public string? ReadingMnemonic { get; set; }
    public string[]? OnYomi { get; set; }
    public string[]? KunYomi { get; set; }
    public string[]? Nanori { get; set; }
    public string[]? VisuallySimilarKanji { get; set; }
    public WaniKaniVocabularyInformation[]? VocabExamples { get; set; }
}