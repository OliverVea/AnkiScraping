using System.Text.Json.Serialization;
using AnkiScraping.Core;

namespace AnkiScraping;

[JsonSerializable(typeof(KanjiInformation))]
public partial class KanjiCardJsonContext : JsonSerializerContext;