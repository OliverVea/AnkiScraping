namespace AnkiScraping.Anki;

internal class AnkiDeckFields
{
    private readonly List<string> _fields = [];
    public IReadOnlyList<string> Fields => _fields;

    public void Clear()
    {
        _fields.Clear();
    }
    
    public void AddField(string field)
    {
        _fields.Add(field);
    }
    
    public void AddFields(params IEnumerable<string> fields)
    {
        _fields.AddRange(fields);
    }
    
    public void WithField(string field)
    {
        _fields.Clear();
        _fields.Add(field);
    }
    
    public void WithFields(params IEnumerable<string> fields)
    {
        _fields.Clear();
        _fields.AddRange(fields);
    }
}