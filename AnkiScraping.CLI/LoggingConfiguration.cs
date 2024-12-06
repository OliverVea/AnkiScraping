namespace AnkiScraping.Host;

public class LoggingConfiguration
{
    public const string Section = "Logging";
    
    public string? LogFile { get; set; }
    public string LogLevel { get; set; } = "Information";
    public bool LogToConsole { get; set; }
}