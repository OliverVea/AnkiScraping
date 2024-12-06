using AnkiScraping.Http;
using AnkiScraping.WaniKani;
using HtmlAgilityPack;
using Moq;
using Serilog;

namespace AnkiScraping.Tests;

public class Tests
{
    private Mock<IHttpService> _httpServiceMock = null!;
    private Mock<ILogger> _loggerMock = null!;
    
    private KanjiScraper _scraper = null!;
    
    [Before(Test)]
    public void Setup()
    {
        _httpServiceMock = new Mock<IHttpService>();
        _loggerMock = new Mock<ILogger>();
        
        _scraper = new KanjiScraper(_httpServiceMock.Object, _loggerMock.Object);
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
        await Assert.That(result).HasMember(x => x.IsT0).EqualTo(true);
        
        var kanjiInformation = result.AsT0;
        
        await Assert.That(kanjiInformation.Kanji).IsEqualTo(kanji);
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