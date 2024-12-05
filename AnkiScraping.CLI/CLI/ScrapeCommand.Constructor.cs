using AnkiScraping.Core;
using Spectre.Console;

namespace AnkiScraping.Host.CLI;

public partial class ScrapeCommand(
    IAnsiConsole console,
    DebugJsonOptions debugJsonOptions,
    IKanjiInformationProvider kanjiInformationProvider,
    CancellationTokenSource cancellationTokenSource)
{
    private CancellationToken CancellationToken => cancellationTokenSource.Token;
}