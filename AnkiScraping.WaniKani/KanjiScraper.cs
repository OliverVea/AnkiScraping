using System.Text.Json;
using AnkiScraping.Http;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace AnkiScraping.WaniKani;

public class WaniKaniScraper(
    IHttpService httpService,
    ILogger<WaniKaniScraper> logger)
{
    private const string WaniKaniKanjiInformationUrl = "https://www.wanikani.com/kanji/{0}";
    
    public async Task<WaniKaniKanjiInformation> ScrapeKanjiInformationAsync(char kanji, CancellationToken ct = default)
    {
        var url = string.Format(WaniKaniKanjiInformationUrl, kanji);
        logger.LogInformation("Fetching WaniKani for kanji: {Kanji}, URL: {Url}", kanji, url);

        HtmlDocument document;
        
        try
        {
            document = await httpService.GetHtml(url, ct);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to fetch WaniKani for kanji: {Kanji}, URL: {Url}", kanji, url);
            throw;
        }
        
        if (logger.IsEnabled(LogLevel.Information))
        {
            var text = document.DocumentNode.TryGetNodeWithClass("body", out var body) ? body.OuterHtml : "Failed to get body";
            
            logger.LogInformation("Fetched WaniKani for kanji: {Kanji}, Document length: {DocumentLength}", kanji, text.Length);
        }
        
        WaniKaniKanjiInformation? information;
        logger.LogInformation("Processing fetched html");
        try
        {
            information = ScrapeInformation(document, kanji);
        }
        catch (Exception e)
        {
            var tempPath = Path.GetTempFileName();
            
            logger.LogError(e, "Failed to scrape WaniKani for kanji: {Kanji}. Writing log to {TempPath}", kanji, tempPath);
            
            var text = document.DocumentNode.OuterHtml;
            await File.WriteAllTextAsync(tempPath, text, ct);
            
            throw;
        }
        
        if (logger.IsEnabled(LogLevel.Information))
        {
            var text = JsonSerializer.Serialize(information, WaniKaniJsonContext.Default.WaniKaniKanjiInformation);
            logger.LogInformation("Scraped WaniKani for kanji: {Kanji}, information: {Information}", kanji, text);
        }
        
        return information;
    }

    private WaniKaniKanjiInformation ScrapeInformation(HtmlDocument document, char kanji)
    {
        var headerText = GetHeaderText(document);
        var radicals = GetRadicals(document);
        var meanings = GetMeanings(document);
        var meaningMnemonic = GetKanjiMnemonic(document);
        var readingMnemonic = GetReadingMnemonic(document);
        var onYomiReadings = GetOnYomiReadings(document);
        var kunYomiReadings = GetKunYomiReadings(document);
        var nanoriReadings = GetNanoriReadings(document);
        var visuallySimilarKanji = GetVisuallySimilarKanji(document);
        var vocabExamples = GetVocabExamples(document);
        
        return new WaniKaniKanjiInformation
        {
            Kanji = kanji,
            HeaderText = headerText,
            Radicals = radicals,
            Meanings = meanings,
            KanjiMnemonic = meaningMnemonic,
            ReadingMnemonic = readingMnemonic,
            OnYomi = onYomiReadings,
            KunYomi = kunYomiReadings,
            Nanori = nanoriReadings,
            VisuallySimilarKanji = visuallySimilarKanji,
            VocabExamples = vocabExamples
        };
    }

    private static string? GetHeaderText(HtmlDocument document)
    {
        return document.DocumentNode.TryGetNodeWithClass(CssClass.PageHeaderTitleText, out var headerTextElement)
            ? headerTextElement.InnerText
            : null;
    }

    private WaniKaniRadicalInformation[]? GetRadicals(HtmlDocument document)
    {
        if (!document.TryGetElementById(ElementId.RadicalSection, out var sectionElement))
        {
            return null;
        }

        var listItems = sectionElement.SelectNodesWithTag(HtmlTags.ListItem);
        
        var radicals = listItems
            .Select(GetRadical)
            .OfType<WaniKaniRadicalInformation>()
            .ToArray();
        
        return radicals;
    }

    private WaniKaniRadicalInformation? GetRadical(HtmlNode listItem)
    {
        if (!listItem.TryGetNodeWithClass(CssClass.SubjectCharacterCharacters, out var characterElement) ||
            !listItem.TryGetNodeWithClass(CssClass.SubjectCharacterMeaning, out var meaningElement))
        {
            logger.LogWarning("Failed to get radical information: listItem={ListItem}", listItem.OuterHtml);
            return null;
        }
        var character = characterElement.InnerText;
        var meaning = meaningElement.InnerText;
        
        if (character == null || meaning == null)
        {
            logger.LogWarning("Failed to get radical information: character={Character}, meaning={Meaning}", character, meaning);
            return null;
        }
        
        return new WaniKaniRadicalInformation
        {
            Radical = character,
            Meaning = meaning
        };
    }

    private string[]? GetMeanings(HtmlDocument document)
    {
        if (!document.TryGetElementById(ElementId.MeaningSection, out var meaningSection))
        {
            return null;
        }
        
        var meaningList = meaningSection.SelectNodesWithClass(CssClass.SubjectSectionMeanings);
        var meanings = meaningList
            .SelectMany(GetMeaningsEntry)
            .OfType<string>()
            .ToArray();
        
        return meanings;
    }
    
    private string?[] GetMeaningsEntry(HtmlNode meaningList)
    {
        if (!meaningList.TryGetNodeWithClass(CssClass.SubjectSectionMeaningsItems, out var element))
        {
            return [];
        }

        var text = element.InnerText;
        
        return SplitList(text);
    }

    
    private string? GetKanjiMnemonic(HtmlDocument document)
    {
        if (!document.TryGetElementById(ElementId.MeaningSection, out var meaningSection))
        {
            return null;
        }
        
        var texts = meaningSection.SelectNodesWithClass(CssClass.SubjectSectionText);
        return string.Join("\n", texts.Select(x => x.InnerText));
    }

    private string? GetReadingMnemonic(HtmlDocument document)
    {
        if (!document.TryGetElementById(ElementId.ReadingSection, out var readingSection))
        {
            return null;
        }
        
        var texts = readingSection.SelectNodesWithClass(CssClass.SubjectSectionText);
        return string.Join("\n", texts.Select(x => x.InnerText));
    }

    
    private string[]? GetReadingWithIndex(HtmlDocument document, int index)
    {
        var readings = document.DocumentNode.SelectNodesWithClass(CssClass.SubjectReadingsReading);
        var readingSection = readings[index];

        if (!readingSection.TryGetNodeWithClass(CssClass.SubjectReadingsReadingItems, out var reading))
        {
            return null;
        }
        
        var text = reading.InnerText.Trim();

        return SplitList(text);
    }

    private string[]? GetOnYomiReadings(HtmlDocument document) => GetReadingWithIndex(document, ReadingIndex.OnYomi);
    private string[]? GetKunYomiReadings(HtmlDocument document) => GetReadingWithIndex(document, ReadingIndex.KunYomi);
    private string[]? GetNanoriReadings(HtmlDocument document) => GetReadingWithIndex(document, ReadingIndex.Nanori);

    private string[]? GetVisuallySimilarKanji(HtmlDocument document)
    {
        if (!document.TryGetElementById(ElementId.VisuallySimilarKanjiSection, out var visuallySimilarSection))
        {
            return null;
        }
        
        var listItems = visuallySimilarSection.SelectNodesWithTag(HtmlTags.ListItem);
        var characters = listItems.Select(x => x.TryGetNodeWithClass(CssClass.SubjectCharacterCharacters, out var item) ? item.InnerText : null).OfType<string>().ToArray();
        
        return characters;
    }

    private WaniKaniVocabularyInformation[]? GetVocabExamples(HtmlDocument document)
    {
        if (!document.TryGetElementById(ElementId.AmalgamationsSection, out var amalgamationsSection))
        {
            return null;
        }
        
        var listItems = amalgamationsSection.SelectNodesWithTag(HtmlTags.ListItem);
        
        var vocabExamples = listItems
            .Select(GetVocabExample)
            .OfType<WaniKaniVocabularyInformation>()
            .ToArray();
        
        return vocabExamples;
    }

    private WaniKaniVocabularyInformation? GetVocabExample(HtmlNode htmlNode)
    {
        if (!htmlNode.TryGetNodeWithClass(CssClass.SubjectCharacterCharacters, out var kanjiElement) ||
            !htmlNode.TryGetNodeWithClass(CssClass.SubjectCharacterReading, out var readingElement) ||
            !htmlNode.TryGetNodeWithClass(CssClass.SubjectCharacterMeaning, out var meaningElement))
        {
            logger.LogWarning("Failed to get vocab example: listItem={ListItem}", htmlNode.OuterHtml);
            return null;
        }
        
        var kanji = kanjiElement.InnerText;
        var meaning = meaningElement.InnerText;
        var reading = readingElement.InnerText;
        
        if (kanji == null || meaning == null || reading == null)
        {
            logger.LogWarning("Failed to get vocab example: kanji={Kanji}, meaning={Meaning}", kanji, meaning);
            return null;
        }

        return new WaniKaniVocabularyInformation(kanji, meaning, reading);
    }


    private static string[] SplitList(string text)
    {
        return text.Split(",").Select(x => x.Trim()).Where(x => !x.Contains("none", StringComparison.InvariantCultureIgnoreCase)).ToArray();
    }

    public static class HtmlTags
    {
        public const string ListItem = "li";
    }

    public static class ElementId
    {
        public const string RadicalSection = "section-components";
        public const string MeaningSection = "section-meaning";
        public const string ReadingSection = "section-reading";
        public const string VisuallySimilarKanjiSection = "section-similar-subjects";
        public const string AmalgamationsSection = "section-amalgamations";
    }

    public static class ReadingIndex
    {
        public const int OnYomi = 0;
        public const int KunYomi = 1;
        public const int Nanori = 2;
    }
    
    public static class CssClass
    {
        public const string PageHeaderTitleText = "page-header__title-text";
        public const string SubjectCharacterCharacters = "subject-character__characters";
        public const string SubjectCharacterMeaning = "subject-character__meaning";
        public const string SubjectCharacterReading = "subject-character__reading";
        public const string SubjectSectionMeanings = "subject-section__meanings";
        public const string SubjectSectionMeaningsItems = "subject-section__meanings-items";
        public const string SubjectSectionText = "subject-section__text";
        public const string SubjectReadingsReading = "subject-readings__reading";
        public const string SubjectReadingsReadingItems = "subject-readings__reading-items";
    }
}
