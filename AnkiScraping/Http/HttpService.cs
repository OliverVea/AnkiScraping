using System.Diagnostics;
using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Hybrid;
using Serilog.Events;

namespace AnkiScraping.Http;

public class HttpService(
    HybridCache hybridCache,
    IHttpClientFactory httpClientFactory,
    ILogger logger) : IHttpService
{
    private ILogger Logger => logger.ForContext<HttpService>();
    
    public async Task<HtmlDocument> GetHtml(string url, CancellationToken ct = default)
    {
        var client = httpClientFactory.CreateClient();

        var cleanedUrl = CleanUrl(url);

        var cacheMiss = false;
        
        var start = Stopwatch.GetTimestamp();
        
        var cachedDocumentText = await hybridCache.GetOrCreateAsync<string>(
            cleanedUrl, 
            async cancellationToken =>
            {
                cacheMiss = true;
                return await GetHtmlDocumentAsync(client, cleanedUrl, cancellationToken);
            },
            cancellationToken: ct);

        var delta = Stopwatch.GetElapsedTime(start);
        Logger.Information("Getting HTML for {Url} took {Time} ms. Cached {Cached}", cleanedUrl, delta, !cacheMiss);
        
        var document = new HtmlDocument();
        document.LoadHtml(cachedDocumentText);
        
        return document;
    }
    
    private async Task<string> GetHtmlDocumentAsync(HttpClient client, string url, CancellationToken ct)
    {
        Logger.Information("Cache miss. Fetching HTML from: {Url}", url);
        
        var response = await client.GetAsync(url, ct); 
        
        return await response.Content.ReadAsStringAsync(ct);
    }
    
    private static string CleanUrl(string url)
    {
        return url.Trim();
    }
}