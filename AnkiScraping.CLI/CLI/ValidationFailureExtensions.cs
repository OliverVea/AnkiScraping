using FluentValidation;
using FluentValidation.Results;
using Spectre.Console;

namespace AnkiScraping.Host.CLI;

public static class ValidationFailureExtensions
{
    public static void Print(this ValidationFailure failure, IAnsiConsole console)
    {
        var color = failure.GetColor();
        console.MarkupLine($"[{color}]{failure.ErrorMessage}[/]");
    }
    
    public static string GetColor(this ValidationFailure failure)
    {
        return failure.Severity switch
        {
            Severity.Error => "red",
            Severity.Warning => "yellow",
            _ => "white"
        };
    }
}