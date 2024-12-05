using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AnkiScraping.Http;

public class HttpService(
    HybridCache hybridCache,
    IHttpClientFactory httpClientFactory,
    ILogger<HttpService> logger) : IHttpService
{
    public async Task<HtmlDocument> GetHtml(string url, CancellationToken ct = default)
    {
        var client = httpClientFactory.CreateClient();

        var cleanedUrl = CleanUrl(url);
        
        var cachedDocumentText = await hybridCache.GetOrCreateAsync<string>(
            cleanedUrl, 
            async cancellationToken => await GetHtmlDocumentAsync(client, cleanedUrl, cancellationToken),
            cancellationToken: ct);
        
        var document = new HtmlDocument();
        document.LoadHtml(cachedDocumentText);
        
        return document;
    }
    
    private async Task<string> GetHtmlDocumentAsync(HttpClient client, string url, CancellationToken ct)
    {
        logger.LogInformation("Fetching HTML from: {Url}", url);
        
        var response = await client.GetAsync(url, ct); 
        
        return await response.Content.ReadAsStringAsync(ct);
    }
    
    private static string CleanUrl(string url)
    {
        return url.Trim();
    }
}