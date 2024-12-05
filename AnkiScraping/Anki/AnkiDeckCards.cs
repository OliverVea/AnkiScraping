namespace AnkiScraping.Anki;

internal class AnkiDeckCards
{
    private readonly List<string[]> _cards = [];
    public IReadOnlyList<string[]> Cards => _cards;
    
    public void Clear()
    {
        _cards.Clear();
    }
    
    public void AddCard(params IEnumerable<string> fields)
    {
        _cards.Add(fields.ToArray());
    }
    
    public void AddCards(params IEnumerable<IEnumerable<string>> cards)
    {
        _cards.AddRange(cards.Select(card => card.ToArray()));
    }
}