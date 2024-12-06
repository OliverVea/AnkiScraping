using AnkiScraping.Anki;
using AnkiScraping.Core.Operations;
using Serilog;
using Spectre.Console;

namespace AnkiScraping.Host.CLI;

public partial class ScrapeCommand(
    IAnsiConsole console,
    GetMultipleKanjiInformationAsyncOperation getMultipleKanjiInformationAsyncOperation,
    GetAnkiCardCollectionOperation getAnkiCardCollectionOperation,
    GetKanjiSetOperation getKanjiSetOperation,
    ILogger logger,
    CancellationTokenSource cancellationTokenSource)
{
    private CancellationToken CancellationToken => cancellationTokenSource.Token;
    private ILogger Logger => logger.ForContext<ScrapeCommand>();
}