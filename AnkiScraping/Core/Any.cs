namespace AnkiScraping.Core.Operations;

public readonly record struct Any
{
    private const string Value = "Any";
    
    public override string ToString()
    {
        return Value;
    }
}