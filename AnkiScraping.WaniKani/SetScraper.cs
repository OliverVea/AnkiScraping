using AnkiScraping.Http;
using HtmlAgilityPack;
using Serilog.Events;

namespace AnkiScraping.WaniKani;

public class SetScraper(IHttpService httpService, ILogger logger)
{
    private const string WaniKaniLevelUrl = "https://www.wanikani.com/level/{0}";

    private ILogger Logger => logger.ForContext<SetScraper>();
    
    public async Task<string> ScrapeKanjiForLevelAsync(int level, CancellationToken ct = default)
    {
        var url = string.Format(WaniKaniLevelUrl, level);
        Logger.Information("Fetching kanji level set from WaniKani. Level: {Level}, URL: {Url}", level, url);

        HtmlDocument document;
        
        try
        {
            document = await httpService.GetHtml(url, ct);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to fetch Level: {Level}, URL: {Url}", level, url);
            throw;
        }
        
        if (Logger.IsEnabled(LogEventLevel.Debug))
        {
            Logger.Debug("Fetched WaniKani level: {Level}, Document: {DocumentLength}", level, document.DocumentNode.OuterHtml);
            return document.DocumentNode.OuterHtml;
        }

        var kanjiChars = document
            .GetElementbyId("kanji")?
            .ParentNode.SelectNodesWithTag("li")
            .Select(x => x
                .SelectNodesWithClass("subject-character__characters")
                .SingleOrDefault()?
                .InnerText[0])
            .OfType<char>();

        if (kanjiChars == null)
        {
            Logger.Warning("Failed to find kanji characters for level: {Level}, URL: {Url}", level, url);
            return string.Empty;
        }
        
        var kanjiSet = new string(kanjiChars.ToArray());
        
        Logger.Information("Found kanji set for level: {Level}, Kanji: {KanjiSet}", level, kanjiSet);
        
        return kanjiSet;
    }
}