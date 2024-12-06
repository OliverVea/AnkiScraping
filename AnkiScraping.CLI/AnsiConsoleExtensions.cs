using Spectre.Console;

namespace AnkiScraping.Host;

public static class AnsiConsoleExtensions
{
    public static void MarkupErrorLine(this IAnsiConsole console, string message)
    {
        console.MarkupLine("[red]{0}[/]", message);
    }
}