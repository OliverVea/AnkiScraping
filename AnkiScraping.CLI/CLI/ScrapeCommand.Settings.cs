using System.ComponentModel;
using System.Text;
using Spectre.Console.Cli;

namespace AnkiScraping.Host.CLI;

public partial class ScrapeCommand
{
    public class Settings : CommandSettings
    {
        [Description("The kanji to scrape information for. Can be specified multiple times. Must be a single character. Can be used in conjunction with -l|--kanji-list.")]
        [CommandOption("-k|--kanji")]
        public required string[] SingleKanji { get; init; } = [];
        
        [Description("The list of kanji to scrape information for. Should only contain the kanji characters with no other characters. Can be used in conjunction with -k|--kanji.")]
        [CommandOption("-l|--kanji-list")]
        [DefaultValue(null)]
        public string? KanjiList { get; init; }
        
        [Description("The name of the output file to write the scraped information to.")]
        [CommandOption("-o|--output")]
        [DefaultValue("output.txt")]
        public required string OutputFile { get; init; }
        
        [Description("Overwrite the output file if it already exists.")]
        [CommandOption("--overwrite")]
        [DefaultValue(false)]
        public bool Overwrite { get; init; }
        
        [Description("Whether to output verbose information.")]
        [CommandOption("-v|--verbose")]
        [DefaultValue(false)]
        public bool Verbose { get; init; }
        
        [Description("Perform a dry run without writing to the output file.")]
        [CommandOption("--dry-run")]
        [DefaultValue(false)]
        public bool DryRun { get; init; }

        public string AllKanji => _allKanji.Value;
        private readonly Lazy<string> _allKanji;

        public Settings()
        {
            _allKanji = new Lazy<string>(GetAllKanji);
        }
        
        private string GetAllKanji()
        {
            var sb = new StringBuilder();
            
            foreach (var kanji in SingleKanji)
            {
                sb.Append(kanji);
            }
            
            if (KanjiList != null)
            {
                sb.Append(KanjiList);
            }
            
            return sb.ToString();
        }
    }
}