using System.Text.Json.Serialization;

namespace AnkiScraping.WaniKani;

[JsonSerializable(typeof(WaniKaniKanjiInformation))]
public partial class WaniKaniJsonContext : JsonSerializerContext;