using System.Text.Json.Serialization;
using AnkiScraping.Core;
using AnkiScraping.WaniKani;

namespace AnkiScraping;

[JsonSerializable(typeof(KanjiCardInformation))]
[JsonSerializable(typeof(WaniKaniKanjiInformation))]
public partial class KanjiCardJsonContext : JsonSerializerContext
{
}