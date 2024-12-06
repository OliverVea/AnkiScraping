namespace AnkiScraping.Core;

/// <summary>
/// The information for a kanji card.
/// </summary>
public class KanjiCardInformation
{
    /// <summary>
    /// The kanji character.
    /// </summary>
    public required Kanji Kanji { get; set; }
    
    
    /// <summary>
    /// The provider ID for the kanji information.
    /// </summary>
    public required  KanjiProviderId ProviderId { get; set; }
    
    /// <summary>
    /// The meanings of the kanji. Listed in order of most common to least common.
    /// </summary>
    public IReadOnlyList<KanjiMeaning>? Meanings { get; set; }
    
    /// <summary>
    /// Onyomi readings. Listed in order of most common to least common.
    /// </summary>
    public IReadOnlyList<OnYomiReading>? OnYomi { get; set; }
    
    /// <summary>
    /// Kunyomi readings. Listed in order of most common to least common.
    /// </summary>
    public IReadOnlyList<KunYomiReading>? KunYomi { get; set; }
    
    /// <summary>
    /// Nanori readings. Listed in order of most common to least common.
    /// </summary>
    public IReadOnlyList<NanoriReading>? Nanori { get; set; }

    /// <summary>
    /// The radicals that make up the kanji. Listed in order of most common to least common.
    /// </summary>
    public IReadOnlyList<RadicalInformation>? Radicals { get; set; }
    
    /// <summary>
    /// Drawing mnemonic for the kanji.
    /// </summary>
    public string? KanjiMnemonic { get; set; }
    
    /// <summary>
    /// Reading mnemonic for the kanji.
    /// </summary>
    public string? ReadingMnemonic { get; set; }
    
    /// <summary>
    /// Example vocab words that use the kanji.
    /// </summary>
    public IReadOnlyList<VocabInformation>? VocabExamples { get; set; }
}

public readonly record struct Kanji(char Character);
public readonly record struct KanjiMeaning(string Meaning);
public readonly record struct RadicalInformation(string Radical, string Meaning);
public readonly record struct VocabInformation(string Kanji, HiraganaString Hiragana, string Meaning);

public readonly record struct KunYomiReading(HiraganaString Reading);
public readonly record struct OnYomiReading(HiraganaString Reading);
public readonly record struct NanoriReading(HiraganaString Reading);

public readonly record struct HiraganaString(string Hiragana);

