using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace AnkiScraping;

public class DebugJsonOptions
{
    public JsonSerializerOptions UnicodeOptions { get; } = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        TypeInfoResolver = KanjiCardJsonContext.Default
    };
}