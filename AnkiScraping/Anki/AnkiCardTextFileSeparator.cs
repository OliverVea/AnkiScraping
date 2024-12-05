namespace AnkiScraping.Anki;

public readonly record struct AnkiCardTextFileSeparator(string Name, string Value)
{
    public static readonly AnkiCardTextFileSeparator Comma = new("Comma", ",");
    public static readonly AnkiCardTextFileSeparator Semicolon = new("Semicolon", ";");
    public static readonly AnkiCardTextFileSeparator Tab = new("Tab", "\t");
    public static readonly AnkiCardTextFileSeparator Space = new("Space", " ");
    public static readonly AnkiCardTextFileSeparator Pipe = new("Pipe", "|");
    public static readonly AnkiCardTextFileSeparator Colon = new("Colon", ":");
}