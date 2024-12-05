using Spectre.Console;
using Spectre.Console.Testing;

namespace AnkiScraping.Tests;

public class CliTests
{
    public const int SuccessfulExitCode = 0;

    public const string ConsoleTest = "console-test";
    
    public const string ScrapeCommand = "scrape";
    public const string SingleKanjiFlag = "-k";
    public const string KanjiListFlag = "-l";

    private TestConsole _testConsole = null!;
    
    [Before(Test)]
    public void SetupConsoleForTest()
    {
        _testConsole = new TestConsole();
        AnsiConsole.Console = _testConsole;
    }
    
    [After(Test)]
    public void TearDownConsoleForTest()
    {
        _testConsole.Dispose();
        AnsiConsole.Console = null!;
    }
    
    [Test]
    public async Task A() => await ScrapeCommand_WithSpecifiedArguments_ShouldFailWithErrorMessage(
        args: [ScrapeCommand], 
        expectedErrorMessage: "At least one kanji character must be specified");
    
    [Test]
    public async Task B() => await ScrapeCommand_WithSpecifiedArguments_ShouldFailWithErrorMessage(
        args: [ScrapeCommand, SingleKanjiFlag], 
        expectedErrorMessage: "Option 'kanji' is defined but no value has been provided.");
    
    [Test]
    public async Task C() => await ScrapeCommand_WithSpecifiedArguments_ShouldFailWithErrorMessage(
        args: [ScrapeCommand, SingleKanjiFlag, "a"], 
        expectedErrorMessage: "Kanji character 'a' is not a valid kanji character");
    
    [Test]
    public async Task D() => await ScrapeCommand_WithSpecifiedArguments_ShouldFailWithErrorMessage(
        args: [ScrapeCommand, SingleKanjiFlag, "日本"], 
        expectedErrorMessage: "Kanji character '日本' must be a single character");
    
    
    private async Task ScrapeCommand_WithSpecifiedArguments_ShouldFailWithErrorMessage(string[] args, string expectedErrorMessage)
    {
        // Act
        var tester = new CommandAppTester();

        var console = new TestConsole();
        
        tester.Configure(config =>
        {
            Host.CLI.ScrapeCommand.AddToConfigurator(config);
            
            config.Settings.Registrar.RegisterInstance<IAnsiConsole>(console);
        });
        
        var result = await tester.RunAsync(args);

        var logDir = "./logoutput";
        var fileName = string.Join(" ", args) + ".txt";
        var filePath = $"{logDir}/{fileName}";
        
        Directory.CreateDirectory(logDir);
        await File.WriteAllTextAsync(filePath, result.Output + $"\n(Exit code: {result.ExitCode})");

        // Assert
        await Assert.That(result.ExitCode).IsNotEqualTo(SuccessfulExitCode);
        await Assert.That(result.Output).Contains(expectedErrorMessage);
    }
}