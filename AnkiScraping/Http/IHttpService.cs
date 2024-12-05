using HtmlAgilityPack;

namespace AnkiScraping.Http;

public interface IHttpService
{
    Task<HtmlDocument> GetHtml(string url, CancellationToken ct = default);
}