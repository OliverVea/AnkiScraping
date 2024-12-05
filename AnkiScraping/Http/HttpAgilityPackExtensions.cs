using System.Diagnostics.CodeAnalysis;
using HtmlAgilityPack;

namespace AnkiScraping.Http;

public static class HttpAgilityPackExtensions
{
    private static readonly IReadOnlyList<HtmlNode> EmptyHtmlNodeList = Array.Empty<HtmlNode>().AsReadOnly();
    
    public static IReadOnlyList<HtmlNode> SelectNodesWithTag(this HtmlNode htmlNode, string tag)
    {
        var xpath = $".//{tag}";
        return htmlNode.SelectNodesWithXPath(xpath);
    }
    
    public static IReadOnlyList<HtmlNode> SelectNodesWithClass(this HtmlNode htmlNode, string cssClass)
    {
        var xpath = $".//*[contains(concat(' ', normalize-space(@class), ' '), ' {cssClass} ')]";
        return htmlNode.SelectNodesWithXPath(xpath);
    }
    
    private static IReadOnlyList<HtmlNode> SelectNodesWithXPath(this HtmlNode htmlNode, string xpath)
    {
        return htmlNode.SelectNodes(xpath)?.AsReadOnly() ?? EmptyHtmlNodeList;
    }
    
    public static bool TryGetElementById(this HtmlDocument document, string id, [NotNullWhen(true)] out HtmlNode? result)
    {
        result = document.GetElementbyId(id);
        return result != null;
    }
    
    public static bool TryGetNodeWithClass(this HtmlNode htmlNode, string cssClass, [NotNullWhen(true)] out HtmlNode? result)
    {
        var results = htmlNode.SelectNodesWithClass(cssClass);
        return TryQuerySelectorInternal(results, out result);
    }
    
    private static bool TryQuerySelectorInternal(IReadOnlyList<HtmlNode> results, [NotNullWhen(true)] out HtmlNode? result)
    {
        if (results.Count != 1)
        {
            result = null;
            return false;
        }
        
        result = results[0];
        return true;
    }
    
    
}