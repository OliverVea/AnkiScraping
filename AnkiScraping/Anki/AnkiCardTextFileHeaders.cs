namespace AnkiScraping.Anki;

/// <summary>
/// https://docs.ankiweb.net/importing/text-files.html#file-headers
/// </summary>
public static class AnkiCardTextFileHeaders
{
    public const string Separator = "separator";
    public const string Html = "html";
    public const string Tags = "tags";
    public const string Columns = "columns";
    public const string NoteType = "notetype";
    public const string Deck = "deck";
    public const string NoteTypeColumn = "notetype column";
    public const string DeckColumn = "deck column";
    public const string TagsColumn = "tags columns";
    public const string GuidColumn = "guid column";
}