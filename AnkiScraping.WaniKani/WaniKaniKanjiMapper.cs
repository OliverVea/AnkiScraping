using AnkiScraping.Core;

namespace AnkiScraping.WaniKani;

public class WaniKaniKanjiMapper
{
    public KanjiInformation Map(WaniKaniKanjiInformation scrapedKanji, ProviderKey<IKanjiInformationProvider> key)
    {
        return new KanjiInformation
        {
            Kanji = new Kanji(scrapedKanji.Kanji),
            ProviderKey = key,
            Meanings = scrapedKanji.Meanings?.Select(x => new KanjiMeaning(x)).ToArray(),
            OnYomi = scrapedKanji.OnYomi?.Select(x => new OnYomiReading(new HiraganaString(x))).ToArray(),
            KunYomi = scrapedKanji.KunYomi?.Select(x => new KunYomiReading(new HiraganaString(x))).ToArray(),
            Radicals = scrapedKanji.Radicals?.Select(x => new RadicalInformation(x.Radical, x.Meaning)).ToArray(),
            KanjiMnemonic = scrapedKanji.KanjiMnemonic,
            ReadingMnemonic = scrapedKanji.ReadingMnemonic,
            VocabExamples = scrapedKanji.VocabExamples?.Select(x => new VocabInformation(x.Kanji, new HiraganaString(x.Hiragana), x.Meaning)).ToArray()
        };
    }
}