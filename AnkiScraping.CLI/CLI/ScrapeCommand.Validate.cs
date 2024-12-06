using FluentValidation;
using Spectre.Console.Cli;

using SpectreResult = Spectre.Console.ValidationResult;
using FluentResult = FluentValidation.Results.ValidationResult;

namespace AnkiScraping.Host.CLI;

public partial class ScrapeCommand
{
    public override SpectreResult Validate(CommandContext context, Settings settings)
    {
        var validator = new Validator();
        var result = validator.Validate(settings);
        
        if (result.IsValid)
        {
            return SpectreResult.Success();
        }
        
        foreach (var error in result.Errors)
        {
            error.Print(console);
        }
        
        return SpectreResult.Error("Validation failed");
    }

    public class Validator : AbstractValidator<Settings>
    {
        public override FluentResult Validate(ValidationContext<Settings> context)
        {
            RuleForEach(x => x.SingleKanji)
                .Length(1)
                .WithMessage("Kanji character '{PropertyValue}' must be a single character. Use -l|--kanji-list to specify multiple characters.");
            
            RuleFor(x => x.OutputFile)
                .NotEmpty()
                .WithMessage("Output file must be specified");
                
            RuleFor(x => File.Exists(x.OutputFile) && !x.Overwrite && !x.DryRun)
                .Equal(false)
                .WithMessage("Output file already exists. Specify a different file name or use --overwrite to overwrite the file");
            
            return base.Validate(context);
        }
    }
}