using AnkiScraping.Http;
using AnkiScraping.WaniKani;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Moq;

namespace AnkiScraping.Tests;

public class Tests
{
    private Mock<IHttpService> _httpServiceMock = null!;
    private Mock<ILogger<WaniKaniScraper>> _loggerMock = null!;
    
    private WaniKaniScraper _scraper = null!;
    
    [Before(Test)]
    public void Setup()
    {
        _httpServiceMock = new Mock<IHttpService>();
        _loggerMock = new Mock<ILogger<WaniKaniScraper>>();
        
        _scraper = new WaniKaniScraper(new DebugJsonOptions(), _httpServiceMock.Object, _loggerMock.Object);
    }
    
    [Test]
    public async Task ScrapeKanjiInformationAsync_WhenKanjiIs九_ReturnsKanjiInformation()
    {
        // Arrange
        var kanji = '九';
        await MockHttpService(kanji);
        
        // Act
        var result = await _scraper.ScrapeKanjiInformationAsync(kanji);
        
        // Assert
        await Assert.That(result.Kanji).IsEqualTo(kanji);
    }
    
    private async Task MockHttpService(char kanji)
    {
        var fileName = $"{kanji}.html";
        var url = $"https://www.wanikani.com/kanji/{kanji}";
        var documentText = await File.ReadAllTextAsync(fileName);
        
        var document = new HtmlDocument();
        document.LoadHtml(documentText);
        
        _httpServiceMock
            .Setup(x => x.GetHtml(url, It.IsAny<CancellationToken>()))
            .ReturnsAsync(document);
    }
}