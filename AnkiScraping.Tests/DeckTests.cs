using AnkiScraping.Anki;

namespace AnkiScraping.Tests;

public class AnkiCardCollectionTests
{

    [Test]
    public async Task CreateDeck()
    {
        var tempFile = Path.GetTempFileName();

        await AnkiCardCollectionBuilder.Create()
            .AddTags("Japanese")
            .WithFields("Kanji", "Meaning", "Onyomi", "Kunyomi", "Radicals", "Radical Mnemonic", "Reading Mnemonic")
            .AddCard("水", "water", "スイ", "みず", "Water is just water (水) also has a small version: 氵", "Water is just water",
                "Hoshimachi Suisei (すい) is a water idol. She often wears a mizugi (水着) swimsuit.")
            .AddCard("\"日\"", "sun", "ニチ", "ひ", "The sun (\"日\") is a circle with a dot in the middle",
                "The sun is a circle with a dot in the middle", "The sun (日) is a circle with a dot in the middle")
            .ExportToCsvAsync(tempFile);
        
        await Assert.That(File.Exists(tempFile)).IsTrue();
        
        File.Delete(tempFile);
    }
}