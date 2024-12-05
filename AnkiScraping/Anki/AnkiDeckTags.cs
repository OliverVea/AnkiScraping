namespace AnkiScraping.Anki;

internal class AnkiDeckTags
{
    private readonly List<string> _tags = [];
    public IReadOnlyList<string> Tags => _tags;
    
    public void Clear()
    {
        _tags.Clear();
    }
    
    public void AddTag(string tag)
    {
        _tags.Add(tag);
    }
    
    public void AddTags(params IEnumerable<string> tags)
    {
        _tags.AddRange(tags);
    }

    public void WithTags(params IEnumerable<string> tags)
    {
        _tags.Clear();
        _tags.AddRange(tags);
    }
}