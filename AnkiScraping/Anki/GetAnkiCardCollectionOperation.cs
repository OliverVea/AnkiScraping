using AnkiScraping.Core;
using Microsoft.Extensions.Logging;

namespace AnkiScraping.Anki;

public class GetAnkiCardCollectionOperation(ILogger<GetAnkiCardCollectionOperation> logger)
{
    private static readonly CardFieldType[] CardFieldOrder = [
        CardFieldType.Meaning,
        CardFieldType.Kanji,
        CardFieldType.Onyomi,
        CardFieldType.Kunyomi,
        CardFieldType.Radicals,
        CardFieldType.ReadingMnemonic,
        CardFieldType.KanjiMnemonic,
        CardFieldType.VocabExamples
    ];
    
    public async Task<IAnkiCardCollectionWithFields> ExecuteAsync(IAsyncEnumerable<KanjiInformation> kanjiInformation, CancellationToken ct = default)
    {
        var cardCollection = AnkiCardCollectionBuilder.Create()
            .WithTags("Japanese", "Kanji")
            .WithFields(CardFieldOrder.Select(GetFieldTypeName).ToArray());

        await foreach (var kanji in kanjiInformation.WithCancellation(ct))
        {
            var cardFields = MapToCardFields(kanji);
            cardCollection.AddCard(cardFields);
            
            logger.LogInformation("Added card for kanji {Kanji}", kanji.Kanji.Character);
        }
        
        logger.LogInformation("Finished adding cards to collection");

        return cardCollection;
    }
    
    private static string[] MapToCardFields(KanjiInformation kanjiInformation)
    {
        return CardFieldOrder.Select(type => GetCardField(type, kanjiInformation)).ToArray();
    }
    
    private static string GetCardField(CardFieldType type, KanjiInformation kanjiInformation)
    {
        return type switch
        {
            CardFieldType.Meaning => string.Join(" / ", kanjiInformation.Meanings?.Select(x => x.Meaning) ?? []),
            CardFieldType.Kanji => kanjiInformation.Kanji.Character.ToString(),
            CardFieldType.Onyomi => string.Join(", ", kanjiInformation.OnYomi?.Select(x => x.Reading.Hiragana) ?? []),
            CardFieldType.Kunyomi => string.Join(", ", kanjiInformation.KunYomi?.Select(x => x.Reading.Hiragana) ?? []),
            CardFieldType.Radicals => string.Join(", ", kanjiInformation.Radicals?.Select(x => $"{x.Meaning} ({x.Radical})") ?? []),
            CardFieldType.ReadingMnemonic => kanjiInformation.ReadingMnemonic ?? "",
            CardFieldType.KanjiMnemonic => kanjiInformation.KanjiMnemonic ?? "",
            CardFieldType.VocabExamples => string.Join(", ", kanjiInformation.VocabExamples?.Take(3).Select(x => x.Hiragana.Hiragana) ?? []),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
    
    private static string GetFieldTypeName(CardFieldType type)
    {
        return type switch
        {
            CardFieldType.Meaning => "Meaning",
            CardFieldType.Kanji => "Kanji",
            CardFieldType.Onyomi => "Onyomi",
            CardFieldType.Kunyomi => "Kunyomi",
            CardFieldType.Radicals => "Radicals",
            CardFieldType.ReadingMnemonic => "Reading Mnemonic",
            CardFieldType.KanjiMnemonic => "Kanji Mnemonic",
            CardFieldType.VocabExamples => "Vocab Examples",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    private enum CardFieldType
    {
        Meaning,
        Kanji,
        Onyomi,
        Kunyomi,
        Radicals,
        ReadingMnemonic,
        KanjiMnemonic,
        VocabExamples
    }
}