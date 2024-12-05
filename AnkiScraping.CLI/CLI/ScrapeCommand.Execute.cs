using System.Text.Json;
using AnkiScraping.Anki;
using AnkiScraping.Core;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;

namespace AnkiScraping.Host.CLI;

public partial class ScrapeCommand : AsyncCommand<ScrapeCommand.Settings>
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
    
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        if (settings.Verbose)
        {
            var kanjiList = string.Join(", ", settings.AllKanji);
            console.MarkupLine($"Scraping information for kanji characters: [bold]{kanjiList}[/]");
        }

        var cardCollection = AnkiCardCollectionBuilder.Create()
            .WithTags("Japanese", "Kanji")
            .WithFields(CardFieldOrder.Select(GetFieldTypeName).ToArray());
        
        foreach (var kanji in settings.AllKanji)
        {
            var kanjiInformation = await kanjiInformationProvider.GetKanjiInformationAsync(kanji, CancellationToken);
            
            if (settings.Verbose)
            {
                console.MarkupLine($"[bold]{kanji}[/]:");
                
#pragma warning disable IL2026
#pragma warning disable IL3050
                var serialized = JsonSerializer.Serialize(kanjiInformation, JsonSerializerOptions.Default);
#pragma warning restore IL3050
#pragma warning restore IL2026
                
                var jsonText = new JsonText(serialized);
                
                console.Write(jsonText);
            }
            
            var cardFields = MapToCardFields(kanjiInformation);
            cardCollection.AddCard(cardFields);
        }

        if (settings.DryRun)
        {
            var text = cardCollection.ExportToString();
            console.MarkupLine($"[bold]Dry run output:[/]\n{text}");
        }
        else
        {
            await cardCollection.ExportToCsvAsync(settings.OutputFile, ct: CancellationToken);
        }
        
        return 0;
    }
    
    private static string[] MapToCardFields(KanjiCardInformation kanjiInformation)
    {
        return CardFieldOrder.Select(type => GetCardField(type, kanjiInformation)).ToArray();
    }
    
    private static string GetCardField(CardFieldType type, KanjiCardInformation kanjiInformation)
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