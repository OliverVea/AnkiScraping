using System.Collections;

namespace AnkiScraping.Core.Operations;

public readonly record struct KanjiSet(IReadOnlyCollection<Kanji> Kanji) : IEnumerable<Kanji>
{
    public override string ToString()
    {
        return string.Join(", ", Kanji);
    }
    
    public IEnumerator<Kanji> GetEnumerator()
    {
        return Kanji.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}